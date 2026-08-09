using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Debug;
using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.IO;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.Recipe;
using SoulboundEngine.Client.Recipe.Asset;
using SoulboundEngine.Client.Render.Block;
using SoulboundEngine.Client.Render.Entity;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Client.Settings;
using SoulboundEngine.Client.UI;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Client.World.Render;
using SoulboundEngine.Client.World.Serialization;
using SoulboundEngine.Client.World.Widget;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Audio;
using SoulboundEngine.Core.Registry;
using SoulboundEngine.Core.Render.Sprite;
using SoulboundEngine.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client {
	using Camera = UnityEngine.Camera;
	using Keyboard = Input.Keyboard;
	using Logger = Debug.Logging.Logger;
	using Object = UnityEngine.Object;
	using RectInt = UnityEngine.RectInt;
	using Vector2 = UnityEngine.Vector2;
	using Vector3 = UnityEngine.Vector3;
#if !UNITY_EDITOR
	using Application = UnityEngine.Application;
	using LogType = UnityEngine.LogType;
	using StackTraceLogType = UnityEngine.StackTraceLogType;
#endif

	public sealed class SoulboundClient : IWorldAccessor, IDebugMetricsSource {
		const int INPUT_QUEUE_BUFFER_CAPACITY = 128;
		const string SAVES_ROOT_FOLDER = "saves";
		private static SoulboundClient instance;
		private readonly GameConfig config;
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
		private readonly WorldSerializer worldSerializer;
		private readonly UIHandler uiHandler;
		private readonly UIAudioEventBank uiAudioEventBank;
		private readonly WorldAudioEventBank worldAudioEventBank;
		private readonly DebugOverlayManager debugOverlayManager;
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
		private WorldScreen activeWorldScreen;
		private PlayerEntity player;
		private WorldSession? activeWorldSession;

		public SoulboundClient(GameConfig config) {
			instance = this;
			this.config = config;
			UXMLSchema_Generated.RegisterAll();

			this.inputActions = new PlayerInputActions();
			this.inputManager = new InputManager(this.inputActions.asset);
			this.clientPlayerInputHandler = new ClientPlayerInputHandler(this);
			this.settings = new GameSettings();

			this.debugMetricsService = new DebugMetricsService();
			this.performanceMetrics = new PerformanceMetrics();
			this.RegisterDebugMetricsSource(this);
			this.runtimeDataProvider = new RuntimeDataProvider();
			this.runtimeExecutionServices = new RuntimeExecutionServices();
			this.worldSessionCommands = new WorldSessionCommands();
			this.commandProcessor = new CommandProcessor(this.runtimeDataProvider, this.runtimeExecutionServices);
			this.debugOverlayManager = new DebugOverlayManager(this);
			this.commandLine = new CommandLine(this.commandProcessor, this);
			this.metricsHud = new MetricsHUD(this.debugMetricsService, this);
			this.logConsole = new LogConsole(this);
#if !UNITY_EDITOR
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
#endif

			File savesFile = UnityPaths.PersistentDataRoot.Combine(SAVES_ROOT_FOLDER);
			this.worldSavesManager = new WorldSavesManager(savesFile, WorldSerializer.SEED_FILE_NAME);
			this.worldSerializer = new WorldSerializer();

			// scene may not be available at this time
			// TODO: change UIHandler init
			this.uiHandler = new UIHandler(Object.FindFirstObjectByType<UIDocument>());

			this.uiAudioEventBank = new UIAudioEventBank();
			this.worldAudioEventBank = new WorldAudioEventBank();
			this.uiAudioEventBank.Activate();
			AudioManager.RebuildPools();

			this.spriteResolver = new AtlasSpriteResolver();
			this.itemRenderManager = new ItemRenderManager(Registries.ITEMS.ToList(), this.spriteResolver);
			this.entityRenderManager = new EntityRenderManager(Registries.ENTITIES.ToList(), this.itemRenderManager);
			this.blockRenderManager = new BlockRenderManager(Registries.BLOCKS.ToList());
			this.worldRenderer = new WorldRenderer(RENDER_RECT, this.blockRenderManager, this.entityRenderManager);
			_ = new InventoryScreens();
			this.worldWidgetManager = new WorldWidgetManager();

			Registry<RecipeIngredientIndex> ingredientIndexRegistry = new(RecipeIngredientIndex.REGISTRY);
			this.recipeManager = new RecipeManager(ingredientIndexRegistry, new RecipeAssetResolver());
		}

		/// <summary>
		/// Called once when the game is launched
		/// </summary>
		internal void Start() {
			this.uiHandler.PushScreen(new TitleScreen(this));
			this.inputManager.Enable();
		}

		/// <summary>
		/// Called once every frame
		/// </summary>
		internal void Update() {
			this.performanceMetrics.Update();
			this.logConsole.Update();
			this.metricsHud.Refresh();
			this.worldRenderer.Render();
		}

		/// <summary>
		/// Called every tick. See <seealso cref="SharedConstants.TICKS_PER_SECOND"/>
		/// </summary>
		internal void Tick() {
			this.HandleInputTick();

			if (this.activeWorldSession is { } session) {
				session.levelManager.Tick();
			}

			// this must be called last, otherwise WasPressed always returns false
			this.inputManager.Tick();
		}

		/// <summary>
		/// Called once when the game is closed
		/// </summary>
		internal void Shutdown() {
			this.activeWorldSession?.levelManager.StopSession();
			this.settings.Save();
			this.inputActions.Dispose();
		}

		private void HandleInputTick() {
			this.metricsHud.Tick();
			this.commandLine.Tick();
			this.logConsole.Tick();

			// gate gameplay by current screen focus
			if (this.uiHandler.HasKeyboardFocus()) {
				return;
			}

			if (this.activeWorldSession is { } worldSession) {
				PlayerEntity player = worldSession.level.GetPlayer();
				this.clientPlayerInputHandler.Handle(player);

				if (this.inputManager.keyboard.WasPressed(Keyboard.GetControl(Key.Escape))) {
					worldSession.levelManager.TogglePause();
				}
			}
		}

		public IScreenHandle OpenScreen(Screen screen) {
			return this.uiHandler.PushScreen(screen);
		}

		public void CloseScreen(IScreenHandle handle) {
			this.uiHandler.PopScreen(handle);
		}

		public void SetInputFocus(IInputFocusable focus) {
			this.uiHandler.SetInputFocus(focus);
		}

		public void ClearInputFocus() {
			this.uiHandler.SetInputFocus(null);
		}

		public void CreateNewWorld(string world, int seed) {
			if (this.config.dev.overrideSaves) {
				seed = this.config.dev.seed;
				world = this.config.dev.devWorld;
			}
			this.worldSavesManager.CreateNewWorld(world, seed, this.worldSerializer);
		}

		public void EnterWorld(string world) {
			if (this.IsWorldSessionActive()) return;

			WorldSave save = this.worldSavesManager.GetSave(world, this.worldSerializer);
			WorldSaveSeedProvider seedProvider = new(save);
			WorldLoader worldLoader = new(this, seedProvider, save, this.worldSerializer);

			this.worldSavesManager.OnWorldEntered(world);
			this.uiHandler.FlushScreens();
			this.worldRenderer.Reset();

			worldLoader.LoadWorld(
				SceneManager.LoadSceneAsync(this.config.unity.worldScene).ToUniTask(),
				Object.FindFirstObjectByType<WorldSceneRoot>
			).ContinueWith(session => {
				this.worldRenderer.SetLevel(session.level);
				this.worldRenderer.SetTilemap(session.tilemap);

				this.player = session.levelManager.StartSession();

				this.activeWorldSession = session;
				this.uiHandler.SetUIDocument(session.uiDocument);
				this.activeWorldScreen = new WorldScreen(this.player.GetInventory(), this.commandLine, this.metricsHud, this.logConsole, this.itemRenderManager);
				this.uiHandler.PushScreen(this.activeWorldScreen);
				this.debugOverlayManager.Clear();

				this.runtimeDataProvider.SetWorldSessionState(session, this.player);
				this.runtimeExecutionServices.SetWorldSessionState(session, this.player);
				this.commandProcessor.RegisterProvider(this.worldSessionCommands);

				// PROTOTYPICAL
				AudioManager.RebuildPools();
				this.worldAudioEventBank.Activate();
			}).Forget(e => Logger.LogFatal(e));
		}

		public void QuitActiveWorld() {
			if (!this.IsWorldSessionActive()) return;

			WorldSession session = this.activeWorldSession.Value;
			LevelManager levelManager = session.levelManager;
			levelManager.StopSession();
			this.worldSerializer.Serialize(levelManager, this.worldSavesManager.ToSaveDirectory(session.save));
			this.player = null;
			this.worldRenderer.SetLevel(null);
			this.uiHandler.FlushScreens();

			SceneManager.LoadSceneAsync(this.config.unity.mainScene).ToUniTask()
				.ContinueWith(() => {
					this.activeWorldSession = null;
					this.uiHandler.SetUIDocument(Object.FindFirstObjectByType<UIDocument>());
					this.activeWorldScreen = null;
					this.uiHandler.PushScreen(new TitleScreen(this));
					this.debugOverlayManager.Clear();

					this.runtimeDataProvider.ExitWorldSessionState();
					this.runtimeExecutionServices.ExitWorldSessionState();
					this.commandProcessor.UnregisterProvider(this.worldSessionCommands);

					// PROTOTYPICAL
					AudioManager.RebuildPools();
					this.worldAudioEventBank.Deactivate();
				})
			.Forget(e => Logger.LogFatal(e));
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

		public Vector2 ScreenToWorldPoint(Vector2 screenPoint) {
			//Canvas canvas = SoulboundClient.Instance.UIHandler.GetCanvas();
			//RectTransform rootTransform = canvas.GetComponent<RectTransform>();
			//bool inWorldPoint = RectTransformUtility.ScreenPointToWorldPointInRectangle(
			//	rootTransform,
			//	screenPos,
			//	Camera.main,
			//	out var worldPoint
			//);
			//if (inWorldPoint) return worldPoint;

			Vector3 pos = screenPoint;
			pos.z = -Camera.main.transform.position.z;
			return Camera.main.ScreenToWorldPoint(pos);
		}

		public WorldWidgetHandle ShowWorldWidget<TContext>(WorldWidgetType<TContext> type, TContext context) where TContext : WorldWidgetContext {
			return this.worldWidgetManager.ShowWidget(type, context);
		}

		public void UpdateWorldWidget<TContext>(WorldWidgetHandle handle, TContext context) where TContext : WorldWidgetContext {
			this.worldWidgetManager.UpdateWidget(handle, context);
		}

		public void DestroyWorldWidget(WorldWidgetHandle handle) {
			this.worldWidgetManager.DestroyWidget(handle);
		}

		public static SoulboundClient Instance => instance;
		public InputManager InputManager => this.inputManager;
		public ItemRenderManager ItemRenderManager => this.itemRenderManager;
		public RecipeManager RecipeManager => this.recipeManager;
		public PerformanceMetrics PerformanceMetrics => this.performanceMetrics;

		public sealed class DebugOverlayManager {
			private readonly Stack<DebugOverlayFeature> overlayStack = new();
			public event Action<DebugOverlayFeature, DebugOverlayFeature> onOverlayChanged;

			public DebugOverlayManager(SoulboundClient client) {
				this.overlayStack.Push(DebugOverlayFeature.None);
			}

			public bool TryShow(DebugOverlayFeature overlay) {
				if (!this.CanShow(overlay)) return false;

				DebugOverlayFeature prev = this.GetActiveOverlay();
				this.overlayStack.Push(overlay);
				onOverlayChanged?.Invoke(prev, this.GetActiveOverlay());
				return true;
			}

			public void Hide(DebugOverlayFeature overlay) {
				if (this.GetActiveOverlay() != overlay) return;
				if (overlay == DebugOverlayFeature.None) return;

				DebugOverlayFeature prev = this.GetActiveOverlay();
				this.overlayStack.Pop();
				onOverlayChanged?.Invoke(prev, this.GetActiveOverlay());
			}

			public void Clear() {
				while (this.GetActiveOverlay() != DebugOverlayFeature.None) {
					this.Hide(this.GetActiveOverlay());
				}
			}

			public DebugOverlayFeature GetActiveOverlay() {
				return this.overlayStack.Peek();
			}

			private bool CanShow(DebugOverlayFeature overlay) => overlay switch {
				DebugOverlayFeature.MetricsHUD => this.GetActiveOverlay() == DebugOverlayFeature.None
					|| this.GetActiveOverlay() == DebugOverlayFeature.CommandLine,
				DebugOverlayFeature.Console => this.GetActiveOverlay() == DebugOverlayFeature.None,
				DebugOverlayFeature.CommandLine => true,
				_ => true
			};
		}

		public enum DebugOverlayFeature {
			None,
			CommandLine,
			MetricsHUD,
			Console
		}
	}
}
