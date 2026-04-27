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

	public sealed class SoulboundClient : IInputEventHandler {
		const int INPUT_QUEUE_BUFFER_CAPACITY = 128;
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

		int IInputEventHandler.priority => int.MaxValue;

		public SoulboundClient(GameConfig config, ClientInit ctx) {
			instance = this;
			this.config = config;

			this.inputActions = new PlayerInputActions();
			this.inputManager = new InputManager(INPUT_QUEUE_BUFFER_CAPACITY, this.inputActions.asset);
			InputTokens.Register(this.inputActions.asset);
			this.settings = new Settings();

			this.runtimeDataProvider = new RuntimeDataProvider();
			this.runtimeExecutionServices = new RuntimeExecutionServices();
			this.commandProcessor = new CommandProcessor(this.runtimeDataProvider, this.runtimeExecutionServices);
			this.worldSessionCommands = new WorldSessionCommands();
			CommandLine commandLine = new(this.commandProcessor);
			MetricsHUD metricsHud = new(ctx.debugMetricsService);
			DebugConsole debugConsole = new();
			this.clientDebug = new ClientDebug(unityLogger, debugConsole, commandLine, metricsHud, this.inputManager);

			// prototypical; will not pass to alpha prod
			var worldSerializer = new JsonSerializer<WorldDump>(Soulbound.globalJsonSettings);
			var worldSerializationPipeline = new SerializationPipeline<WorldDump>(worldSerializer);
			var service = new WorldSerializationService(this.GetWorldSaveStrategy(), worldSerializationPipeline);
			this.worldManager = new WorldManager(service);

			// scene may not be available at this time
			// TODO: change UIHandler init
			this.uiHandler = new UIHandler(Object.FindFirstObjectByType<Canvas>());
			this.inputManager.AddHandler(this.uiHandler);

			this.uiAudioEventBank = new UIAudioEventBank();
			this.worldAudioEventBank = new WorldAudioEventBank();
			this.uiAudioEventBank.Activate();
			AudioManager.RebuildPools();
		}

		/// <summary>
		/// called once when the game is launched
		/// </summary>
		public void Start() {
			this.uiHandler.SetScreen(new TitleScreen());
			this.inputManager.AddHandler(this);
		}

		/// <summary>
		/// called every frame
		/// </summary>
		public void Update() {
			this.inputManager.DispatchInputs();
		}

		public void CreateNewWorld(string world) {
			this.worldManager.CreateNewWorld(world);
		}

		public void EnterWorld(string world) {
			if (this.IsWorldSessionActive()) return;

			if (!this.worldManager.ListSaves().Any(s => s == world) && !this.config.dev.useDoNotSaveWorldStrategy) {
				throw new ArgumentException($"World not found: '{world}'");
			}

			this.uiHandler.FlushScreens();

			// manual dev seed for prototyping
			DevSeedProvider seedProvider = new(this.config.dev);
			WorldLoader worldLoader = new(seedProvider);

			worldLoader.LoadWorld(
				SceneManager.LoadSceneAsync("WorldScene").ToUniTask(),
				Object.FindFirstObjectByType<WorldSceneRoot>
			).ContinueWith(session => {
				this.activeWorldSession = session;
				this.uiHandler.SetCanvas(session.canvas);
				this.uiHandler.SetScreen(new WorldScreen(session.player));
				this.inputManager.AddHandler(session.levelManager);

				this.runtimeDataProvider.SetWorldSessionState(session);
				this.runtimeExecutionServices.SetWorldSessionState(session);
				this.commandProcessor.RegisterProvider(this.worldSessionCommands);

				// PROTOTYPICAL
				AudioManager.RebuildPools();
				this.worldAudioEventBank.Activate();
			}).Forget(e => Logger.LogFatal(e));
		}

		public void QuitActiveWorld() {
			if (!this.IsWorldSessionActive()) return;

			LevelManager levelManager = this.activeWorldSession?.levelManager!;
			levelManager.StopSession();
			this.inputManager.RemoveHandler(levelManager);
			this.uiHandler.FlushScreens();
			Time.timeScale = 1f;

			SceneManager.LoadSceneAsync("DevScene").ToUniTask()
				.ContinueWith(() => {
					this.activeWorldSession = null;
					this.uiHandler.SetCanvas(Object.FindFirstObjectByType<Canvas>());
					this.uiHandler.SetScreen(new TitleScreen());

					this.runtimeDataProvider.ExitWorldSessionState();
					this.runtimeExecutionServices.ExitWorldSessionState();
					this.commandProcessor.UnregisterProvider(this.worldSessionCommands);

					// PROTOTYPICAL
					AudioManager.RebuildPools();
					this.worldAudioEventBank.Deactivate();
				})
			.Forget(e => Logger.LogFatal(e));
		}

		public IEnumerable<string> ListWorldSaves() {
			return this.worldManager.ListSaves();
		}

		public bool IsWorldSessionActive() => this.activeWorldSession != null;

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			return new InputEventListener[] {
				InputEventListener.ConsumePerformed(InputTokens.Debug.toggleMetrics, _ => {
					this.activeWorldSession?.level.ToggleChunkFeatures();
					this.clientDebug.ToggleMetricsHUD();
				}),
				InputEventListener.ConsumePerformed(InputTokens.Debug.enterCommand, _ => this.clientDebug.ShowCommandLine()),
				InputEventListener.ConsumePerformed(InputTokens.Debug.toggleConsole, _ => this.clientDebug.ToggleConsole())
			};
		}

		public void Shutdown() {
			this.activeWorldSession?.levelManager.StopSession();
			this.settings.Save();
			this.inputActions.Dispose();
		}

		[Obsolete]
		private IWorldSaveStrategy GetWorldSaveStrategy() {
#if !UNITY_EDITOR
			return new WorldSaveStrategy(config.file.savesFolder, Application.persistentDataPath);
#else
			return !this.config.dev.useDoNotSaveWorldStrategy
				? new WorldSaveStrategy(this.config.file.savesFolder, Application.persistentDataPath)
				: new DoNotSaveWorldStrategy();
#endif
		}

		public static SoulboundClient Instance => instance;
		public InputManager InputManager => this.inputManager;
		public UIHandler UIHandler => this.uiHandler;

	}
}
