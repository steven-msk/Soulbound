namespace SoulboundEngine.Client {
	using Cysharp.Threading.Tasks;
	using SoulboundEngine.Client.Assets;
	using SoulboundEngine.Client.Audio;
	using SoulboundEngine.Client.Debug;
	using SoulboundEngine.Client.Debug.Commands;
	using SoulboundEngine.Client.Debug.Logging;
	using SoulboundEngine.Client.Debug.Logging.Console;
	using SoulboundEngine.Client.Debug.Metrics;
	using SoulboundEngine.Client.Debug.Metrics.View;
	using SoulboundEngine.Client.Input;
	using SoulboundEngine.Client.IO;
	using SoulboundEngine.Client.Recipe.Asset;
	using SoulboundEngine.Client.Render.Block;
	using SoulboundEngine.Client.Render.Entity;
	using SoulboundEngine.Client.Render.Item;
	using SoulboundEngine.Client.Render.Sprite;
	using SoulboundEngine.Client.Render.World;
	using SoulboundEngine.Client.Settings;
	using SoulboundEngine.Client.UI;
	using SoulboundEngine.Client.UI.Screen;
	using SoulboundEngine.Client.UI.UXMLBindings;
	using SoulboundEngine.Client.World;
	using SoulboundEngine.Client.World.Widget;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.GameStates;
	using SoulboundEngine.Recipe;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;
	using SoulboundEngine.World.Serialization;
	using SoulboundEngine.World.Services;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using UnityEditor;
	using UnityEngine.InputSystem;
	using UnityEngine.SceneManagement;
	using UnityEngine.UIElements;
	using Camera = UnityEngine.Camera;
	using Keyboard = Input.Keyboard;
	using Object = UnityEngine.Object;
	using RectInt = UnityEngine.RectInt;
	using Vector2 = UnityEngine.Vector2;
	using Vector3 = UnityEngine.Vector3;

#nullable enable

	public sealed class SoulboundClient : IWorldAccessor, IDebugMetricsSource {
		private static SoulboundClient instance = null!;
		private static readonly UnityClientLoggerWrapper clientLoggerWrapper = new(UnityEngine.Debug.unityLogger);
		private readonly Stopwatch tickStopwatch = new();
		private readonly Stopwatch tpsWindowStopwatch = new();
		public const float TICK_RATE = 1f / SharedConstants.TICKS_PER_SECOND;
		private readonly ClientConfig config;
		private readonly PlayerInputActions inputActions;
		private readonly InputManager inputManager;
		private readonly ClientPlayerInputHandler clientPlayerInputHandler;
		private readonly GameSettings settings;
		private readonly LogConsole logConsole;
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHud;
		private readonly CommandProcessor commandProcessor;
		private readonly WorldSessionCommands worldSessionCommands;
		private readonly RuntimeDataProvider runtimeDataProvider;
		private readonly RuntimeExecutionServices runtimeExecutionServices;
		private readonly WorldSavesManager worldSavesManager;
		private readonly WorldSaveValidator worldSerializer;
		private readonly UIHandler uiHandler;
		private readonly UIAudioEventBank uiAudioEventBank;
		private readonly WorldAudioEventBank worldAudioEventBank;
		private readonly ItemRenderManager itemRenderManager;
		private readonly ISpriteResolver<AtlasSpriteRef> spriteResolver;
		private readonly EntityRenderManager entityRenderManager;
		private readonly BlockRenderManager blockRenderManager;
		private readonly WorldRenderer worldRenderer;
		public static readonly RectInt RENDER_RECT = new(-32, -19, 65, 39);
		private readonly RecipeManager recipeManager;
		private readonly PerformanceMetrics performanceMetrics;
		private readonly DebugMetricsService debugMetricsService;
		private readonly WorldWidgetManager worldWidgetManager;
		private bool running;
		private int ticksThisSecond;
		private WorldScreen? activeWorldScreen;
		private PlayerEntity? player;
		private WorldSession? activeWorldSession;
		private IScreenHandle? pauseScreenHandle;

		[UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void GameLaunch() {
			new SoulboundClient(Main.instance.GetClientConfig()).Start();
		}

		private SoulboundClient(ClientConfig config) {
			instance = this;
			this.config = config;
			GameStateManager.SetBootstrapping();

			Logger.SetWrapper(clientLoggerWrapper);
			UXMLSchema_Generated.RegisterAll();

			this.logConsole = new LogConsole(this);

			AssetManager.LoadAllWithPreloadLabel();

			Registries.Init();
			Registries.Freeze();

			this.inputActions = new PlayerInputActions();
			this.inputManager = new InputManager(this.inputActions.asset);
			this.clientPlayerInputHandler = new ClientPlayerInputHandler(this);
			this.settings = new GameSettings();

			File savesFile = UnityPaths.PersistentDataRoot.Combine(config.file.savesRoot);
			this.worldSavesManager = new WorldSavesManager(savesFile);
			this.worldSerializer = new WorldSaveValidator(config.file.seedFile, config.file.chunksFolder);

			this.debugMetricsService = new DebugMetricsService();
			this.performanceMetrics = new PerformanceMetrics();
			this.RegisterDebugMetricsSource(this);
			this.runtimeDataProvider = new RuntimeDataProvider();
			this.runtimeExecutionServices = new RuntimeExecutionServices();
			this.worldSessionCommands = new WorldSessionCommands();
			this.commandProcessor = new CommandProcessor(this.runtimeDataProvider, this.runtimeExecutionServices);
			this.commandLine = new CommandLine(this.commandProcessor, this);
			this.metricsHud = new MetricsHUD(this.debugMetricsService, this);
			this.uiHandler = new UIHandler(this.commandLine, this.logConsole, this.metricsHud);

			this.uiAudioEventBank = new UIAudioEventBank();
			this.worldAudioEventBank = new WorldAudioEventBank();
			this.uiAudioEventBank.Activate();
			AudioManager.RebuildPools();

			this.spriteResolver = new AtlasSpriteResolver();
			this.itemRenderManager = new ItemRenderManager(Registries.ITEMS.ToList(), this.spriteResolver);
			this.entityRenderManager = new EntityRenderManager(Registries.ENTITIES.ToList(), this.itemRenderManager);
			this.blockRenderManager = new BlockRenderManager(Registries.BLOCKS.ToList());
			this.worldWidgetManager = new WorldWidgetManager(Registries.WORLD_WIDGET_TYPE);
			this.worldRenderer = new WorldRenderer(RENDER_RECT, this.blockRenderManager, this.entityRenderManager, this.worldWidgetManager);
			_ = new InventoryScreens();

			Registry<RecipeIngredientIndex> ingredientIndexRegistry = new(RecipeIngredientIndex.REGISTRY);
			this.recipeManager = new RecipeManager(ingredientIndexRegistry, new RecipeAssetResolver());

			GameStateManager.SetInitialized();
		}

		public void Start() {
			if (this.running) return;
			GameStateManager.SetLaunching();

			UnityEngine.Application.quitting += this.OnApplicationQuit;

			this.running = true;
			// not safe UIDocument resolution
			// TODO: rework UIHandler init with UIDocument resolution
			this.uiHandler.SetUIDocument(Object.FindFirstObjectByType<UIDocument>());
			this.uiHandler.PushScreen(new TitleScreen(this));
			this.inputManager.Enable();

			UniTask.Post(this.FrameLoop);
			UniTask.Post(this.TickLoop);
			GameStateManager.SetRunning();
		}

		private void Update() {
			this.performanceMetrics.Update();
			this.logConsole.Update();
			this.metricsHud.Refresh();
			this.worldRenderer.Render();
		}

		private async void TickLoop() {
			while (this.running) {
				this.StartTick();
				try {
					this.Tick();
				} catch (Exception e) {
					Logger.LogFatal(e);
					if (this.config.isRunningInEditor) {
						EditorApplication.isPlaying = false;
					} else {
						Environment.FailFast("Uncaught exception in tick loop", e);
					}
				}
				this.EndTick();
				await UniTask.WaitForSeconds(TICK_RATE, true);
			}
		}

		private async void FrameLoop() {
			while (this.running) {
				try {
					this.Update();
				} catch (Exception e) {
					// TODO: custom crash handling
					Logger.LogFatal(e);
					if (this.config.isRunningInEditor) {
						EditorApplication.isPlaying = false;
					} else {
						Environment.FailFast("Uncaught exception in frame loop", e);
					}
				}
				await UniTask.NextFrame();
			}
		}

		private void Tick() {
			this.HandleInputTick();

			if (this.activeWorldSession is { } session) {
				session.levelManager.Tick();
			}

			// this must be called last, otherwise WasPressed always returns false
			this.inputManager.Tick();
		}

		private void StartTick() {
			this.tickStopwatch.Restart();
		}

		private void EndTick() {
			this.tickStopwatch.Stop();
			double elapsedMs = this.tickStopwatch.Elapsed.TotalMilliseconds;
			if (elapsedMs > TICK_RATE * 1000f * 1.5f) {
				Logger.LogWarning($"Tick lag detected! Tick took {elapsedMs:F1}ms (target {TICK_RATE * 1000F}ms)");
			}

			this.ticksThisSecond++;
			if (this.tpsWindowStopwatch.Elapsed.TotalSeconds >= 1.0d) {
				if (this.ticksThisSecond < SharedConstants.TICKS_PER_SECOND - 1) {
					Logger.LogWarning("Tick rate degraded: {} TPS (target {})", this.ticksThisSecond, SharedConstants.TICKS_PER_SECOND);
				}
				this.ticksThisSecond = 0;
				this.tpsWindowStopwatch.Restart();
			}
		}

		private void OnApplicationQuit() {
			GameStateManager.SetShutdown();
			this.Shutdown();
			GameStateManager.SetTerminated();
		}

		private void Shutdown() {
			AssetManager.Shutdown();
			this.activeWorldSession?.levelManager.StopSession();
			this.settings.Save();
			this.inputActions.Dispose();
		}

		public void Close() => UnityEngine.Application.Quit();

		private void HandleInputTick() {
			// known issue: screen key presses are desynced with this method
			// due to UIToolkit dispatching input the frame it was invoked
			bool hasKeyboardFocus = this.uiHandler.HasKeyboardFocus();
			bool isPointerOverUI = this.uiHandler.IsPointerOverUI();
			this.metricsHud.Tick();
			this.commandLine.Tick();
			this.logConsole.Tick();

			if (this.activeWorldSession is { } worldSession) {
				LevelManager levelManager = worldSession.levelManager;
				Level level = worldSession.level;
				bool isPaused = worldSession.levelManager.paused;

				PlayerEntity player = level.GetPlayer();
				this.clientPlayerInputHandler.Handle(player,
					shouldBlockKeyboardActions: hasKeyboardFocus || isPaused,
					shouldBlockMouse: isPointerOverUI || isPaused
				);

				if (!hasKeyboardFocus && this.inputManager.keyboard.WasPressed(Keyboard.GetControl(Key.Escape))) {
					if (!levelManager.paused) {
						this.PauseGame();
					} else {
						this.UnpauseGame();
					}
				}
			}
		}

		public IScreenHandle OpenScreen(Screen screen) {
			return this.uiHandler.PushScreen(screen);
		}

		public void CloseScreen(IScreenHandle handle) {
			this.uiHandler.PopScreen(handle);
		}

		public void PushInputFocus(IInputFocusable focus) => this.uiHandler.PushInputFocus(focus);
		public void PopInputFocus(IInputFocusable focus) => this.uiHandler.PopInputFocus(focus);

		public void CreateNewWorld(string world, int seed) {
			if (this.config.dev.overrideSaves) {
				seed = this.config.dev.seed;
				world = this.config.dev.devWorld;
			}
			this.worldSavesManager.CreateNewWorld(world, seed, this.worldSerializer);
		}

		public void EnterWorld(string world) {
			if (this.IsWorldSessionActive()) return;

			this.worldRenderer.Reset();
			this.metricsHud.Hide();

			WorldSave save = this.worldSavesManager.GetSave(world, this.worldSerializer);
			WorldSaveSeedProvider seedProvider = new(save);
			ClientWorldBootstrapper worldLoader = new(seedProvider, save);

			UniTask<WorldBootData> worldBootTask = worldLoader.LoadWorld(this.recipeManager);
			UniTask sceneLoadTask = SceneManager.LoadSceneAsync(this.config.unity.worldScene, LoadSceneMode.Additive).ToUniTask();

			UniTask.WhenAll(worldBootTask, sceneLoadTask)
				.ContinueWith(() => {
					WorldSceneRoot sceneRoot = Object.FindFirstObjectByType<WorldSceneRoot>();
					if (!sceneRoot) {
						throw new InvalidOperationException("Root provider does not exist");
					}
					this.worldSavesManager.OnWorldEntered(world);
					this.uiHandler.FlushScreens();
					this.worldRenderer.Reset();

					SceneManager.UnloadSceneAsync(this.config.unity.mainScene);
					WorldBootData bootData = worldBootTask.GetAwaiter().GetResult();

					WorldSession session = new() {
						save = save,
						level = bootData.level,
						levelManager = bootData.levelManager,
						canvas = sceneRoot.canvas,
						uiDocument = sceneRoot.UIDocument,
						tilemap = sceneRoot.tilemap
					};

					this.player = session.levelManager.StartSession(level => new ClientPlayerEntity(this, level));

					this.worldRenderer.SetLevel(session.level);
					this.worldRenderer.SetTilemap(session.tilemap);

					this.activeWorldSession = session;
					this.uiHandler.SetUIDocument(session.uiDocument);
					this.activeWorldScreen = new WorldScreen(this.player.GetInventory(), this.itemRenderManager);
					this.uiHandler.PushScreen(this.activeWorldScreen);

					this.runtimeDataProvider.SetWorldSessionState(session, this.player);
					this.runtimeExecutionServices.SetWorldSessionState(session, this.player);
					this.commandProcessor.RegisterProvider(this.worldSessionCommands);

					// PROTOTYPICAL
					AudioManager.RebuildPools();
					this.worldAudioEventBank.Activate();
				})
			.Forget(e => throw e);
		}

		public void QuitActiveWorld() {
			if (!this.IsWorldSessionActive()) return;

			WorldSession session = this.activeWorldSession!.Value;
			LevelManager levelManager = session.levelManager;
			levelManager.StopSession();
			this.player = null;
			this.worldRenderer.SetLevel(null);
			this.ShowChunkFeatures(false);
			this.metricsHud.Hide();
			this.uiHandler.FlushScreens();

			SceneManager.LoadSceneAsync(this.config.unity.mainScene).ToUniTask()
				.ContinueWith(() => {
					this.activeWorldSession = null;
					this.uiHandler.SetUIDocument(Object.FindFirstObjectByType<UIDocument>());
					this.activeWorldScreen = null;
					this.uiHandler.PushScreen(new TitleScreen(this));

					this.runtimeDataProvider.ExitWorldSessionState();
					this.runtimeExecutionServices.ExitWorldSessionState();
					this.commandProcessor.UnregisterProvider(this.worldSessionCommands);

					// PROTOTYPICAL
					AudioManager.RebuildPools();
					this.worldAudioEventBank.Deactivate();
				})
			.Forget(e => throw e);
		}

		public void PauseGame() {
			if (this.activeWorldSession is not { } session) return;
			if (session.levelManager.paused) return;

			session.levelManager.PauseGame();
			if (this.pauseScreenHandle != null) this.CloseScreen(this.pauseScreenHandle);
			this.pauseScreenHandle = this.OpenScreen(new GamePausedScreen(this));
		}

		public void UnpauseGame() {
			if (this.activeWorldSession is not { } session) return;
			if (!session.levelManager.paused) return;

			session.levelManager.UnpauseGame();
			if (this.pauseScreenHandle != null) this.CloseScreen(this.pauseScreenHandle!);
			this.pauseScreenHandle = null;
		}

		public IEnumerable<WorldSave> ListWorldSaves() {
			return this.worldSavesManager.ListSaves(this.worldSerializer);
		}

		public void DeleteWorld(string world) {
			this.worldSavesManager.DeleteWorld(world);
		}

		public bool IsWorldSessionActive() => this.activeWorldSession != null;

		public void ShowChunkFeatures(bool showChunkFeatures) {
			if (showChunkFeatures) this.worldRenderer.ShowChunkFeatures();
			else this.worldRenderer.HideChunkFeatures();
		}

		void IDebugMetricsSource.CollectDebugData(ref DebugMetricsBuilder builder) {
			PerformanceMetrics metrics = this.performanceMetrics;
			builder.Add(DebugMetricId.Fps, metrics.InstantFps);
			builder.Add(DebugMetricId.FrameTime, metrics.FrameTime);
			builder.Add(DebugMetricId.FixedUpdateTime, metrics.FixedUpdateTime);
			builder.Add(DebugMetricId.TotalManagedMemory, metrics.TotalManagedMemoryMB);
			builder.Add(DebugMetricId.TotalUnityReservedMemory, metrics.TotalUnityReservedMemoryMB);
			builder.Add(DebugMetricId.MonoHeap, metrics.MonoHeapMB);
			builder.Add(DebugMetricId.MonoUsed, metrics.MonoUsedMB);
			builder.Add(DebugMetricId.GpuManagedMemory, metrics.GPUManagedMemoryMB);
			builder.Add(DebugMetricId.GpuReservedMemory, metrics.GPUReservedMemoryMB);
			builder.Add(DebugMetricId.GcAlloc, metrics.GcAllocBytesThisFrame);
		}

		public void RegisterDebugMetricsSource(IDebugMetricsSource source) {
			this.debugMetricsService.RegisterSource(source);
		}
		public void UnregisterDebugMetricsSource(IDebugMetricsSource source) {
			this.debugMetricsService.UnregisterSource(source);
		}

		public static int GetRandomWorldSeed() {
			return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		}

		public Vec2d ScreenToWorldPoint(Vector2 screenPoint) {
			Vector3 pos = screenPoint;
			pos.z = -Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint(pos);
			return new Vec2d(pos.x, pos.y);
		}

		[Obsolete] public WorldSession? GetActiveWorldSession() => this.activeWorldSession;

		public static SoulboundClient Instance => instance;
		public InputManager InputManager => this.inputManager;
		public ItemRenderManager ItemRenderManager => this.itemRenderManager;
		public RecipeManager RecipeManager => this.recipeManager;
		public PerformanceMetrics PerformanceMetrics => this.performanceMetrics;
	}
}
