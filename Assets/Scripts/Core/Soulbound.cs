using SoulboundBackend.Client;
using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Structure.Templates;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

#nullable enable

namespace SoulboundBackend.Core {
	public sealed class Soulbound {
		public static Soulbound instance { get; private set; } = null!;
		private readonly GameConfig config;
		public readonly WorldManager worldManager;
		public readonly Settings settings;
		public readonly PlayerInputActions playerInputActions;

		public Soulbound(GameConfig config) {
			instance = this;
			this.config = config;
			this.settings = new Settings();
			this.playerInputActions = new PlayerInputActions();
			IWorldSaveStrategy saveStrategy = config.dev.loadDevWorldFromSave
				? new WorldSaveStrategy(config.file.savesFolder, Application.persistentDataPath)
				: new DoNotSaveWorldStrategy();
			this.worldManager = new WorldManager(config.file.savesFolder, saveStrategy);
			worldManager.onWorldLoaded += (levelManager, dump) => {
				levelManager.SpawnPlayer(dump?.player);
			};
		}

		public void Prototype_LoadDevWorld() {
			Level.RegisterStructure(TreeStructure.instance);
			string world = config.dev.loadDevWorldFromSave
				? config.dev.devWorld
				: $"altw_{Guid.NewGuid()}";
			worldManager.LoadWorld(world,
				GameObject.FindFirstObjectByType<SceneContext>,
				() => SceneManager.LoadScene("WorldScene")
			);
		}

		public void OnApplicationQuit() {
			settings.Save();
		}

		public Level? GetActiveLevel() {
			return GetActiveLevelManager()?.level;
		}

		public LevelManager? GetActiveLevelManager() {
			return worldManager.activeLevelManager;
		}

		[PROTOTYPICAL]
		public PlayerController? GetPlayerInstance() {
			return worldManager.activeLevelManager?.player;
		}
	}
}
