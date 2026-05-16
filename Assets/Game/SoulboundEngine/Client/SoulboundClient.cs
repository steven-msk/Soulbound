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
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Client.World.Serialization;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Audio;
using SoulboundEngine.Core.Serialization;
using System;
using System.Linq;

namespace SoulboundEngine.Client {
	using SoulboundEngine.Client.Render.Block;
	using SoulboundEngine.Client.Render.Entity;
	using SoulboundEngine.Client.Render.Item;
	using SoulboundEngine.Client.UI.Screen;
	using SoulboundEngine.Core.Registry;
	using SoulboundEngine.Core.Render.Sprite;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using UnityEngine.UIElements;
	using Application = UnityEngine.Application;
	using Object = UnityEngine.Object;
	using Time = UnityEngine.Time;

	public sealed class SoulboundClient : IInputEventHandler, IWorldAccessor {
		const int INPUT_QUEUE_BUFFER_CAPACITY = 128;
		private static SoulboundClient instance;
		private readonly GameConfig config;
		private readonly PlayerInputActions inputActions;
		private readonly InputManager inputManager;
		private readonly Settings settings;
		private readonly LogConsole logConsole;
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHud;
		private readonly CommandProcessor commandProcessor;
		private readonly WorldSessionCommands worldSessionCommands;
		private readonly RuntimeDataProvider runtimeDataProvider;
		private readonly RuntimeExecutionServices runtimeExecutionServices;
		private readonly WorldManager worldManager;
		private readonly UIHandler uiHandler;
		private readonly UIAudioEventBank uiAudioEventBank;
		private readonly WorldAudioEventBank worldAudioEventBank;
		private readonly DebugOverlayManager debugOverlayManager;
		private WorldSession? activeWorldSession;
		private readonly ItemRenderManager itemRenderManager;
		private readonly ISpriteResolver<AtlasSpriteRef> spriteResolver;
		private readonly EntityRenderManager entityRenderManager;
		private readonly BlockRenderManager blockRenderManager;

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
			this.worldSessionCommands = new WorldSessionCommands();
			this.commandProcessor = new CommandProcessor(this.runtimeDataProvider, this.runtimeExecutionServices);
			this.debugOverlayManager = new DebugOverlayManager(this);
			this.commandLine = new CommandLine(this.commandProcessor, this.debugOverlayManager);
			this.metricsHud = new MetricsHUD(ctx.debugMetricsService);
			this.logConsole = new LogConsole();

			// prototypical; will not pass to alpha prod
			var worldSerializer = new JsonSerializer<WorldDump>(Soulbound.globalJsonSettings);
			var worldSerializationPipeline = new SerializationPipeline<WorldDump>(worldSerializer);
			var service = new WorldSerializationService(this.GetWorldSaveStrategy(), worldSerializationPipeline);
			this.worldManager = new WorldManager(service);

			// scene may not be available at this time
			// TODO: change UIHandler init
			this.uiHandler = new UIHandler(Object.FindFirstObjectByType<UIDocument>());
			this.inputManager.AddHandler(this.uiHandler);

			this.uiAudioEventBank = new UIAudioEventBank();
			this.worldAudioEventBank = new WorldAudioEventBank();
			this.uiAudioEventBank.Activate();
			AudioManager.RebuildPools();

			this.spriteResolver = new AtlasSpriteResolver();
			this.itemRenderManager = new ItemRenderManager(Registries.ITEMS.ToList(), this.spriteResolver);
			this.entityRenderManager = new EntityRenderManager(Registries.ENTITIES.ToList(), this.itemRenderManager);
			this.blockRenderManager = new BlockRenderManager(Registries.BLOCKS.ToList());
		}

		/// <summary>
		/// called once when the game is launched
		/// </summary>
		public void Start() {
			this.uiHandler.SetScreen(new TitleScreen(this));
			this.inputManager.AddHandler(this);
		}

		/// <summary>
		/// called once every frame
		/// </summary>
		public void Update() {
			this.inputManager.DispatchInputs();
			this.metricsHud.Refresh();
			this.logConsole.Update();
		}

		/// <summary>
		/// called once when the game is closed
		/// </summary>
		public void Shutdown() {
			this.activeWorldSession?.levelManager.StopSession();
			this.settings.Save();
			this.inputActions.Dispose();
		}

		public void CreateNewWorld(string world, int seed) {
			if (this.config.dev.overrideSaves) {
				seed = this.config.dev.seed;
				world = this.config.dev.devWorld;
			}
			this.worldManager.CreateNewWorld(world, seed);
		}

		public void EnterWorld(string world) {
			if (this.IsWorldSessionActive()) return;
			
			WorldSave? save = this.worldManager.ListSaves().FirstOrDefault(s => s.name == world);
			if (save == null) {
				throw new ArgumentException($"World not found: '{world}'");
			}

			this.uiHandler.FlushScreens();

			SeedProvider seedProvider = new(save.GetValueOrDefault());
			WorldLoader worldLoader = new(this, this.entityRenderManager, this.blockRenderManager, seedProvider);

			worldLoader.LoadWorld(
				SceneManager.LoadSceneAsync(this.config.unity.worldScene).ToUniTask(),
				Object.FindFirstObjectByType<WorldSceneRoot>
			).ContinueWith(session => {
				this.activeWorldSession = session;
				//this.uiHandler.SetCanvas(session.canvas);
				this.uiHandler.SetUIDocument(session.uiDocument);
				this.uiHandler.SetScreen(new WorldScreen(this.itemRenderManager, session.player, this.commandLine, this.metricsHud, this.logConsole));
				this.debugOverlayManager.Clear();
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

			SceneManager.LoadSceneAsync(this.config.unity.mainScene).ToUniTask()
				.ContinueWith(() => {
					this.activeWorldSession = null;
					this.uiHandler.SetUIDocument(Object.FindFirstObjectByType<UIDocument>());
					this.uiHandler.SetScreen(new TitleScreen(this));
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
			return this.worldManager.ListSaves();
		}

		public void DeleteWorld(string world) {
			this.worldManager.DeleteWorld(world);
		}

		public bool IsWorldSessionActive() => this.activeWorldSession != null;

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			return new InputEventListener[] {
				InputEventListener.ConsumePerformed(InputTokens.Debug.toggleMetrics, _ => {
					if (!this.metricsHud.isVisible && this.debugOverlayManager.TryShow(DebugOverlayFeature.MetricsHUD)) {
						this.metricsHud.Show();
						this.activeWorldSession?.level.ShowChunkFeatures();
					} else if (this.metricsHud.isVisible) {
						this.metricsHud.Hide();
						this.activeWorldSession?.level.HideChunkFeatures();
						this.debugOverlayManager.Hide(DebugOverlayFeature.MetricsHUD);
					}
				}),

				InputEventListener.ConsumePerformed(InputTokens.Debug.enterCommand, _ => {
					if (this.debugOverlayManager.TryShow(DebugOverlayFeature.CommandLine)) {
						this.commandLine.Show();
						this.activeWorldSession?.player.StopHorizontalMovement();
					}
				}),
				InputEventListener.ConsumePerformed(InputTokens.Debug.toggleConsole, _ => {
					if (!this.logConsole.isVisible && this.debugOverlayManager.TryShow(DebugOverlayFeature.Console)) {
						this.logConsole.Show();
					} else if (this.logConsole.isVisible) {
						this.logConsole.Hide();
						this.debugOverlayManager.Hide(DebugOverlayFeature.Console);
					}
				})
			};
		}

		private IWorldSaveStrategy GetWorldSaveStrategy() {
			return new WorldSaveStrategy(this.config.file.savesFolder, Application.persistentDataPath);
		}

		public static SoulboundClient Instance => instance;
		[Obsolete]
		public InputManager InputManager => this.inputManager;
		[Obsolete]
		public UIHandler UIHandler => this.uiHandler;

		public sealed class DebugOverlayManager {
			private readonly Stack<DebugOverlayFeature> overlayStack = new();
			public event Action<DebugOverlayFeature, DebugOverlayFeature> onOverlayChanged;

			public DebugOverlayManager(SoulboundClient client) {
				this.overlayStack.Push(DebugOverlayFeature.None);

				onOverlayChanged += (prev, next) => {
					if (client.activeWorldSession is { } session) {
						if (client.commandLine.isVisible || next == DebugOverlayFeature.CommandLine) {
							client.inputManager.RemoveHandler(session.player);
						} else if (!client.commandLine.isVisible && prev == DebugOverlayFeature.CommandLine) {
							client.inputManager.AddHandler(session.player);
						}
					}
				};
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
