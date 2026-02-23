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
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Core {
	public sealed class Soulbound : IApplicationController {
		public static Soulbound instance { get; private set; } = null!;
		private bool running;
		private readonly GameConfig config;
		private readonly UIHandler uiHandler;
		private readonly WorldManager worldManager;
		public readonly Settings settings;
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
			settings = new Settings();
			InputTokens.Register(inputActions.asset);
			inputManager = new InputManager(inputActions.asset);

#if !UNITY_EDITOR
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
#endif
			SoulboundDebug debug = new(UnityEngine.Debug.unityLogger);
			inputManager.PushContext(debug.GetConsole());

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
			return !config.dev.useDoNotSaveWorldStrategy
				? new WorldSaveStrategy(config.file.savesFolder, Application.persistentDataPath)
				: new DoNotSaveWorldStrategy();
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

		public InputManager GetInputManager() => inputManager;
		public InputActionAsset GetInputActionAsset() => inputActions.asset;
	}
}
