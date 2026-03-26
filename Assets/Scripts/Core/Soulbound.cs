using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SoulboundBackend.Client.Debug;
using SoulboundBackend.Client.Debug.Commands;
using SoulboundBackend.Client.Debug.Metrics;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Players;
using SoulboundBackend.Client.Runtime.Services;
using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Client.World.Generation;
using SoulboundBackend.Client.World.LevelDomain;
using SoulboundBackend.Client.World.Serialization;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Core.Assets;
using SoulboundBackend.Core.Audio;
using SoulboundBackend.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = SoulboundBackend.Client.Debug.Logging.Logger;


#nullable enable

namespace SoulboundBackend.Core {
	public sealed class Soulbound : IApplicationController, IDebugMetricsSource {
		public static Soulbound instance { get; private set; } = null!;
		private static readonly PlayerInputActions inputActions = new();
		public static readonly JsonSerializerSettings globalJsonSettings = new() {
			TypeNameHandling = TypeNameHandling.Auto,
			TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
			Converters = new List<JsonConverter> {
				new Vector2JsonConverter(),
				new Vector3JsonConverter(),
				new ColorJsonConverter()
			},
		};
		private bool running;
		private readonly GameConfig config;
		private readonly PerformanceMetrics performanceMetrics;
		private readonly UIHandler uiHandler;
		private readonly WorldManager worldManager;
		public readonly Settings settings;
		private readonly RuntimeDataProvider runtimeDataProvider = new();
		private readonly RuntimeExecutionServices runtimeExecutionServices = new();
		private readonly CommandProcessor commandProcessor;
		private readonly WorldSessionCommands worldSessionCommands = new();
		private readonly DebugMetricsService debugMetricsService;
		private readonly SoulboundDebug debug;
		private readonly InputManager inputManager;
		private WorldSession? activeWorldSession;

		private AudioEventBank audioEventBank = new();

		public Soulbound(GameConfig config) {
			instance = this;
			this.config = config;

			inputManager = new InputManager(inputActions.asset);
			InputTokens.Register(inputActions.asset);
			settings = new Settings();

			commandProcessor = new CommandProcessor(runtimeDataProvider, runtimeExecutionServices);
			debugMetricsService = new DebugMetricsService();
			debug = new SoulboundDebug(UnityEngine.Debug.unityLogger, debugMetricsService, commandProcessor);
			inputManager.PushContext(debug);
			RegisterDebugMetricsSource(this);
			performanceMetrics = new PerformanceMetrics();

			AssetManager.PreloadAll();

			var worldSerializer = new JsonSerializer<WorldDump>(globalJsonSettings);
			var worldSerializationPipeline = new SerializationPipeline<WorldDump>(worldSerializer);
			var service = new WorldSerializationService(GetWorldSaveStrategy(), worldSerializationPipeline);
			worldManager = new WorldManager(service);

			// scene may not be available at this time
			uiHandler = new UIHandler(UnityEngine.Object.FindFirstObjectByType<Canvas>());
			inputManager.PushContext(uiHandler);

			RuntimeHelpers.RunClassConstructor(typeof(EntityType).TypeHandle);
			RuntimeHelpers.RunClassConstructor(typeof(Items).TypeHandle);
			RuntimeHelpers.RunClassConstructor(typeof(Blocks).TypeHandle);
		}

		public void Launch() {
			if (running) return;
			running = true;

			try {
				Thread.CurrentThread.Name = "LaunchThread";
			} catch (InvalidOperationException) {
			}

			Application.quitting += ((IApplicationController)this).OnApplicationQuit;

			uiHandler.SetScreen(new TitleScreen());
		}

		public void FrameTick() {
			performanceMetrics.Tick();
			inputManager.DispatchInputs();
		}

		public void CreateNewWorld(string world) {
			worldManager.CreateNewWorld(world);
		}

		public void EnterWorld(string world) {
			if (IsWorldSessionActive())

			if (!worldManager.ListSaves().Any(s => s == world) && !config.dev.useDoNotSaveWorldStrategy) {
				throw new ArgumentException($"World not found: '{world}'");
			}

			uiHandler.FlushScreens();

			// manual dev seed for prototyping
			DevSeedProvider seedProvider = new(config.dev);
			LevelLoader levelLoader = new(seedProvider);

			levelLoader.LoadLevel(
				SceneManager.LoadSceneAsync("WorldScene").ToUniTask(),
				UnityEngine.Object.FindFirstObjectByType<WorldSceneRoot>
			).ContinueWith(session => {
				activeWorldSession = session;
				uiHandler.SetCanvas(session.canvas);
				uiHandler.SetScreen(new WorldScreen(session.player));
				inputManager.PushContext(session.levelManager);

				runtimeDataProvider.SetWorldSessionState(session);
				runtimeExecutionServices.SetWorldSessionState(session);
				commandProcessor.RegisterProvider(worldSessionCommands);

				// PROTOTYPICAL
				AudioManager.InitOneShot(session.audioSource);
				audioEventBank.Activate();
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
					uiHandler.SetCanvas(UnityEngine.Object.FindFirstObjectByType<Canvas>());
					uiHandler.SetScreen(new TitleScreen());

					runtimeDataProvider.ExitWorldSessionState();
					runtimeExecutionServices.ExitWorldSessionState();
					commandProcessor.UnregisterProvider(worldSessionCommands);

					// PROTOTYPICAL
					audioEventBank.Deactivate();
				})
			.Forget(e => Logger.LogFatal(e));
		}

		void IApplicationController.OnApplicationQuit() {
			activeWorldSession?.levelManager.StopSession();
			settings.Save();
			inputActions.Dispose();
			AssetManager.Shutdown();
		}

		private IWorldSaveStrategy GetWorldSaveStrategy() {
#if !UNITY_EDITOR
			return new WorldSaveStrategy(config.file.savesFolder, Application.persistentDataPath);
#else
			return !config.dev.useDoNotSaveWorldStrategy
				? new WorldSaveStrategy(config.file.savesFolder, Application.persistentDataPath)
				: new DoNotSaveWorldStrategy();
#endif
		}

		public bool IsWorldSessionActive() => activeWorldSession != null;

		public UIHandler GetUIHandler() => uiHandler;


		[PROTOTYPICAL]
		[Obsolete]
		public Player? GetPlayerInstance() {
			return activeWorldSession?.player;
		}

		public IEnumerable<string> ListWorldSaves() {
			return worldManager.ListSaves();
		}

		public void RegisterDebugMetricsSource(IDebugMetricsSource source) {
			debugMetricsService.RegisterSource(source);
		}
		public void UnregisterDebugMetricsSource(IDebugMetricsSource source) {
			debugMetricsService.UnregisterSource(source);
		}

		void IDebugMetricsSource.CollectDebugData(ref DebugMetricsBuilder builder) {
			builder.Add("fps", performanceMetrics.InstantFps);
			builder.Add("frameTime", performanceMetrics.FrameTime);
			builder.Add("fixedUpdateTime", performanceMetrics.FixedUpdateTime);
			builder.Add("totalManagedMemory", performanceMetrics.TotalManagedMemoryMB);
			builder.Add("totalUnityReservedMemory", performanceMetrics.TotalUnityReservedMemoryMB);
			builder.Add("monoHeap", performanceMetrics.MonoHeapMB);
			builder.Add("monoUsed", performanceMetrics.MonoUsedMB);
			builder.Add("gpuManagedMemory", performanceMetrics.GPUManagedMemoryMB);
			builder.Add("gpuReservedMemory", performanceMetrics.GPUReservedMemoryMB);
			builder.Add("gcAlloc", performanceMetrics.GcAllocBytesThisFrame);
		}

		public PerformanceMetrics GetPerformanceMetrics() => performanceMetrics;
		public InputManager GetInputManager() => inputManager;
	}
}
