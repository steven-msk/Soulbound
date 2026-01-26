using Cysharp.Threading.Tasks;
using SoulboundBackend.Client;
using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.World;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Common.Logging;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using SoulboundBackend.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

#nullable enable

namespace SoulboundBackend.Core {
	public sealed class Soulbound {
		public static Soulbound instance { get; private set; } = null!;
		private readonly GameConfig config;
		private readonly UIHandler uiHandler;
		public readonly WorldManager worldManager;
		public readonly Settings settings;
		public readonly PlayerInputActions playerInputActions;
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
			playerInputActions = new PlayerInputActions();
			var worldSerializer = new JsonSerializer<WorldDump>(globalJsonSettings);
			var worldSerializationPipeline = new SerializationPipeline<WorldDump>(worldSerializer);
			var service = new WorldSerializationService(GetWorldSaveStrategy(), worldSerializationPipeline);
			worldManager = new WorldManager(service);

			// scene may not be available at this time
			uiHandler = new UIHandler(UnityEngine.Object.FindFirstObjectByType<Canvas>());
		}

		public void Run() {
			uiHandler.SetScreen(new TitleScreen());
		}

		[PROTOTYPICAL]
		public void Prototype_LoadDevWorld() {
			LoadWorld(config.dev.devWorld);
		}

		// aware of the problem with world deserialization

		[PROTOTYPICAL]
		public void LoadWorld(string world) {
			uiHandler.FlushScreens();

			// prototypical
			worldManager.LoadWorld(world,
				SceneManager.LoadSceneAsync("WorldScene"),
				seed: config.dev.seed,
				() => UnityEngine.Object.FindFirstObjectByType<WorldSceneRoot>()
			).Forget(Debug.LogException);
		}

		public void OnApplicationQuit() {
			settings.Save();
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
	}
}
