using Cysharp.Threading.Tasks;
using SoulboundBackend.Client.World;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World {
	public sealed class WorldManager {
		public LevelManager? activeLevelManager { get; private set; }
		private readonly WorldSerializationService serializationService;

		public WorldManager(WorldSerializationService serializationService) {
			this.serializationService = serializationService;
		}

		public IEnumerable<string> ListSaves() {
			string savesRoot = serializationService.GetSavesRoot();
			if (!Directory.Exists(savesRoot)) {
				yield break;
			}

			foreach (var dir in Directory.GetDirectories(savesRoot)) {
				if (File.Exists(Path.Combine(dir, LevelManager.worldDump))) {
					yield return Path.GetFileName(dir);
				}
			}
		}

		public void CreateNewWorld(string world) {
			if (ListSaves().Any(s => s == world)) {
				throw new ArgumentException($"World with name '{world}' already exists");
			}

			string savesRoot = serializationService.GetSavesRoot();
			string worldPath = Path.Combine(savesRoot, world);
			var dir = Directory.CreateDirectory(worldPath);
			var filePath = Path.Combine(worldPath, LevelManager.worldDump);
			File.WriteAllText(filePath, string.Empty);
		}

		public async UniTask<WorldSession> LoadWorld(
				string world,
				int seed,		// placeholder
				UniTask sceneLoadTask,
				Func<IWorldSceneRoot> rootGetter
			) {
			WorldDump? dump = null;
			try {
				if (!serializationService.Load(world, out dump)) {
					throw new ArgumentException($"Could not load dump for world '{world}'");
				}
			} catch (Exception e) {
				UnityEngine.Debug.LogException(e);
			}

			// placeholder
			seed = dump.HasValue ? dump.Value.seed : seed;

			LevelManager levelManager = await LoadWorldSceneAsync(sceneLoadTask, rootGetter);
			activeLevelManager = levelManager;

			activeLevelManager.BootstrapWorld(world, dump, seed, rootGetter().CreateGridContext());
			Player player = activeLevelManager.SpawnPlayer();

			return new WorldSession {
				deserializationData = dump,
				player = player,
				levelManager = levelManager,
				level = levelManager.level
			};
		}

		public async UniTask<LevelManager> LoadWorldSceneAsync(UniTask sceneLoadTask, Func<IWorldSceneRoot> rootGetter) {
			await sceneLoadTask;

			var root = rootGetter()
				?? throw new InvalidOperationException("Failed to load world scene: missing root");

			root.sceneContext.Install();
			root.sceneContext.Resolve();

			return root.sceneContext.Container.Resolve<LevelManager>();
		}

		public void SaveWorld(LevelManager levelManager) {
			serializationService.Save(levelManager.CreateDump(), levelManager.world);
		}

		public void QuitActiveSession() {
			if (activeLevelManager == null) {
				return;
			}

			activeLevelManager.StopSession();
			SaveWorld(activeLevelManager);
		}

		public bool IsSessionActive() => activeLevelManager != null;
	}
}
