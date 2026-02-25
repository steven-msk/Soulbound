using Cysharp.Threading.Tasks;
using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.World;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Core.Debug;
using SoulboundBackend.Core.Resource;
using SoulboundBackend.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;
using SoulboundBackend.Core.Debug.Commands;

#nullable enable

namespace SoulboundBackend.Core {
	public sealed class Soulbound : IApplicationController, IDebugMetricsSource {
		public static Soulbound instance { get; private set; } = null!;
		private bool running;
		private readonly GameConfig config;
		private readonly PerformanceMetrics performanceMetrics;
		private readonly UIHandler uiHandler;
		private readonly WorldManager worldManager;
		public readonly Settings settings;
		private readonly IDynamicDataProvider dynamicDataProvider;
		private readonly CommandProcessor commandProcessor;
		private readonly ICommandProvider commandProvider;
		private readonly DebugMetricsService debugMetricsService;
		private readonly SoulboundDebug debug;
		private readonly InputManager inputManager;
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

		public Soulbound(GameConfig config) {
			instance = this;
			this.config = config;

			inputManager = new InputManager(inputActions.asset);
			InputTokens.Register(inputActions.asset);
			settings = new Settings();

			dynamicDataProvider = new DynamicDataProvider();
			commandProvider = new CommandProvider();
			commandProcessor = new CommandProcessor(commandProvider, dynamicDataProvider);
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

		// aware of the problem with world deserialization

		public void CreateNewWorld(string world) {
			worldManager.CreateNewWorld(world);
		}

		public void EnterWorld(string world) {
			if (worldManager.IsSessionActive()) return;

			if (!worldManager.ListSaves().Any(s => s == world) && !config.dev.useDoNotSaveWorldStrategy) {
				throw new ArgumentException($"World not found: '{world}'");
			}

			uiHandler.FlushScreens();

			worldManager.LoadWorld(world, config.dev.seed,
				SceneManager.LoadSceneAsync("WorldScene").ToUniTask(),
				UnityEngine.Object.FindFirstObjectByType<WorldSceneRoot>
			).ContinueWith(session => {
				uiHandler.SetScreen(new WorldScreen(session.player));
			}).Forget(UnityEngine.Debug.LogException);
		}

		public void QuitActiveWorld() {
			if (!worldManager.IsSessionActive()) return;

			worldManager.QuitActiveSession();
			uiHandler.FlushScreens();
			Time.timeScale = 1f;

			SceneManager.LoadSceneAsync("DevScene").ToUniTask()
				.ContinueWith(() => {
					uiHandler.SetCanvas(UnityEngine.Object.FindFirstObjectByType<Canvas>());
					uiHandler.SetScreen(new TitleScreen());
				})
			.Forget(UnityEngine.Debug.LogException);
		}

		public bool IsWorldSessionActive() {
			return worldManager.IsSessionActive();
		}

		void IApplicationController.OnApplicationQuit() {
			worldManager.QuitActiveSession();
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

		public UIHandler GetUIHandler() => uiHandler;

		[PROTOTYPICAL]
		[Obsolete]
		public Level? GetActiveLevel() {
			return GetActiveLevelManager()?.level;
		}

		[PROTOTYPICAL]
		[Obsolete]
		public LevelManager? GetActiveLevelManager() {
			return worldManager.activeLevelManager;
		}

		[PROTOTYPICAL]
		[Obsolete]
		public PlayerController? GetPlayerInstance() {
			return worldManager.activeLevelManager?.player;
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

		public PerformanceMetrics GetPerformanceMetrics() => performanceMetrics;
		public InputManager GetInputManager() => inputManager;
		public InputActionAsset GetInputActionAsset() => inputActions.asset;
		public IDynamicDataProvider GetDynamicDataProvider() => dynamicDataProvider;
	}
}
