using Cysharp.Threading.Tasks;
using SoulboundBackend.Client.Input;
using SoulboundEngine.Client.Debug;
using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Client.SettingSystem;
using SoulboundEngine.Client.UI;
using SoulboundEngine.Client.UI.Screens;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Generation;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Client.World.Serialization;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Audio;
using SoulboundEngine.Core.Serialization;
using System;
using System.Linq;

namespace SoulboundEngine.Client {
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using static UnityEngine.Debug;
	using Application = UnityEngine.Application;
	using Canvas = UnityEngine.Canvas;
	using Object = UnityEngine.Object;
	using Time = UnityEngine.Time;

	public sealed class SoulboundClient {
		private static SoulboundClient instance;
		private readonly GameConfig config;
		private readonly PlayerInputActions inputActions;
		private readonly InputManager inputManager;
		private readonly Settings settings;
		private readonly ClientDebug clientDebug;
		private readonly CommandProcessor commandProcessor;
		private readonly WorldSessionCommands worldSessionCommands = new();
		private readonly RuntimeDataProvider runtimeDataProvider;
		private readonly RuntimeExecutionServices runtimeExecutionServices;
		private readonly WorldManager worldManager;
		private readonly UIHandler uiHandler;
		private readonly UIAudioEventBank uiAudioEventBank;
		private readonly WorldAudioEventBank worldAudioEventBank;
		private WorldSession? activeWorldSession;

		public SoulboundClient(GameConfig config, ClientInit ctx) {
			instance = this;
			this.config = config;

			inputActions = new PlayerInputActions();
			inputManager = new InputManager(inputActions.asset);
			InputTokens.Register(inputActions.asset);
			settings = new Settings();

			runtimeDataProvider = new RuntimeDataProvider();
			runtimeExecutionServices = new RuntimeExecutionServices();
			commandProcessor = new CommandProcessor(runtimeDataProvider, runtimeExecutionServices);
			worldSessionCommands = new WorldSessionCommands();
			CommandLine commandLine = new(commandProcessor);
			MetricsHUD metricsHud = new(ctx.debugMetricsService);
			DebugConsole debugConsole = new();
			clientDebug = new ClientDebug(unityLogger, debugConsole, commandLine, metricsHud);
			inputManager.PushContext(clientDebug);

			// prototypical; will not pass to alpha prod
			var worldSerializer = new JsonSerializer<WorldDump>(Soulbound.globalJsonSettings);
			var worldSerializationPipeline = new SerializationPipeline<WorldDump>(worldSerializer);
			var service = new WorldSerializationService(GetWorldSaveStrategy(), worldSerializationPipeline);
			worldManager = new WorldManager(service);

			// scene may not be available at this time
			// TODO: change UIHandler init
			uiHandler = new UIHandler(Object.FindFirstObjectByType<Canvas>());
			inputManager.PushContext(uiHandler);

			uiAudioEventBank = new UIAudioEventBank();
			worldAudioEventBank = new WorldAudioEventBank();
			uiAudioEventBank.Activate();
			AudioManager.RebuildPools();
		}

		/// <summary>
		/// called once when the game is launched
		/// </summary>
		public void Start() {
			uiHandler.SetScreen(new TitleScreen());
		}

		/// <summary>
		/// called every frame
		/// </summary>
		public void Update() {
			inputManager.DispatchInputs();
		}

		public void CreateNewWorld(string world) {
			worldManager.CreateNewWorld(world);
		}

		public void EnterWorld(string world) {
			if (IsWorldSessionActive()) return;

			if (!worldManager.ListSaves().Any(s => s == world) && !config.dev.useDoNotSaveWorldStrategy) {
				throw new ArgumentException($"World not found: '{world}'");
			}

			uiHandler.FlushScreens();

			// manual dev seed for prototyping
			DevSeedProvider seedProvider = new(config.dev);
			WorldLoader worldLoader = new(seedProvider);

			worldLoader.LoadWorld(
				SceneManager.LoadSceneAsync("WorldScene").ToUniTask(),
				Object.FindFirstObjectByType<WorldSceneRoot>
			).ContinueWith(session => {
				activeWorldSession = session;
				uiHandler.SetCanvas(session.canvas);
				uiHandler.SetScreen(new WorldScreen(session.player));
				inputManager.PushContext(session.levelManager);

				runtimeDataProvider.SetWorldSessionState(session);
				runtimeExecutionServices.SetWorldSessionState(session);
				commandProcessor.RegisterProvider(worldSessionCommands);

				// PROTOTYPICAL
				AudioManager.RebuildPools();
				worldAudioEventBank.Activate();
			}).Forget(e => Logger.LogFatal(e));
		}

		public void QuitActiveWorld() {
			if (!IsWorldSessionActive()) return;

			LevelManager levelManager = activeWorldSession?.levelManager!;
			levelManager.StopSession();
			inputManager.RemoveContext(levelManager);
			uiHandler.FlushScreens();
			Time.timeScale = 1f;

			SceneManager.LoadSceneAsync("DevScene").ToUniTask()
				.ContinueWith(() => {
					activeWorldSession = null;
					uiHandler.SetCanvas(Object.FindFirstObjectByType<Canvas>());
					uiHandler.SetScreen(new TitleScreen());

					runtimeDataProvider.ExitWorldSessionState();
					runtimeExecutionServices.ExitWorldSessionState();
					commandProcessor.UnregisterProvider(worldSessionCommands);

					// PROTOTYPICAL
					AudioManager.RebuildPools();
					worldAudioEventBank.Deactivate();
				})
			.Forget(e => Logger.LogFatal(e));
		}

		public IEnumerable<string> ListWorldSaves() {
			return worldManager.ListSaves();
		}

		public bool IsWorldSessionActive() => activeWorldSession != null;

		public void Shutdown() {
			activeWorldSession?.levelManager.StopSession();
			settings.Save();
			inputActions.Dispose();
		}

		[Obsolete]
		private IWorldSaveStrategy GetWorldSaveStrategy() {
#if !UNITY_EDITOR
			return new WorldSaveStrategy(config.file.savesFolder, Application.persistentDataPath);
#else
			return !config.dev.useDoNotSaveWorldStrategy
				? new WorldSaveStrategy(config.file.savesFolder, Application.persistentDataPath)
				: new DoNotSaveWorldStrategy();
#endif
		}

		public static SoulboundClient Instance => instance;
		public InputManager InputManager => inputManager;
		public UIHandler UIHandler => uiHandler;
	}
}
