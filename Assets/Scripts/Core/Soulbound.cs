using SoulboundBackend.Client;
using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.World;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Json;
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
			this.settings = new Settings();
			this.playerInputActions = new PlayerInputActions();
			IWorldSaveStrategy saveStrategy = config.dev.loadDevWorldFromSave
				? new WorldSaveStrategy(config.file.savesFolder, Application.persistentDataPath)
				: new DoNotSaveWorldStrategy();
			ISerializer<WorldDump> worldSerializer = new JsonSerializer<WorldDump>(globalJsonSettings);
			SerializationPipeline<WorldDump> worldSerializationPipeline = new(worldSerializer);
			this.worldManager = new WorldManager(new WorldSerializationService(saveStrategy, worldSerializationPipeline));
			this.uiHandler = new UIHandler(GameObject.FindFirstObjectByType<Canvas>());
		}

		public void Run() {

			// this will be removed later
			// as the play tests will explode in time
			// if the player spawns automatically
			worldManager.onWorldLoaded += (levelManager, dump) => {
				levelManager.SpawnPlayer(dump?.player);
			};

			uiHandler.SetScreen(new TitleScreen());
		}

		public void Prototype_LoadDevWorld() {
			string world = config.dev.loadDevWorldFromSave
				? config.dev.devWorld
				: $"altw_{Guid.NewGuid()}";

			uiHandler.DisposeScreenStack();

			worldManager.LoadWorld(world,
				GameObject.FindFirstObjectByType<SceneContext>,
				() => SceneManager.LoadScene("WorldScene")
			);
		}

		public void OnApplicationQuit() {
			settings.Save();
		}

		public UIHandler GetUIHandler() => uiHandler;

		[PROTOTYPICAL]
		public Level? GetActiveLevel() {
			return GetActiveLevelManager()?.level;
		}

		[PROTOTYPICAL]
		public LevelManager? GetActiveLevelManager() {
			return worldManager.activeLevelManager;
		}

		[PROTOTYPICAL]
		public PlayerController? GetPlayerInstance() {
			return worldManager.activeLevelManager?.player;
		}
	}
}
