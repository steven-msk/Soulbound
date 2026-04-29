using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Client.World.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World {
	public sealed class WorldManager {
		private readonly WorldSerializationService serializationService;

		public WorldManager(WorldSerializationService serializationService) {
			this.serializationService = serializationService;
		}

		public IEnumerable<WorldSave> ListSaves() {
			string savesRoot = this.serializationService.GetSavesRoot();
			if (!Directory.Exists(savesRoot)) yield break;

			foreach (var dir in Directory.GetDirectories(savesRoot)) {
				if (File.Exists(Path.Combine(dir, LevelManager.worldDump))) {
					string worldName = Path.GetFileName(dir);
					string seedPath = this.GetSeedPath(dir);

					int seed = GetRandomSeed();
					try {
						seed = int.Parse(File.ReadAllText(seedPath));
					} catch (FileNotFoundException) {
						Logger.LogError("Could not find seed file for save '{}'. Fallback to random seed", worldName);
						this.CreateSeedPath(dir, seed);
					}
					yield return new WorldSave(worldName, seed);
				}
			}
		}

		public void CreateNewWorld(string world, int seed) {
			if (this.ListSaves().Any(s => s.name == world)) {
				throw new ArgumentException($"World with name '{world}' already exists");
			}

			string savesRoot = this.serializationService.GetSavesRoot();
			string savePath = Path.Combine(savesRoot, world);

			Directory.CreateDirectory(savePath);
			string dumpPath = Path.Combine(savePath, LevelManager.worldDump);
			File.WriteAllText(dumpPath, string.Empty);

			this.CreateSeedPath(savePath, seed);
		}

		private void CreateSeedPath(string savePath, int seed) {
			string seedPath = this.GetSeedPath(savePath);
			File.WriteAllText(seedPath, seed.ToString());
		}

		private string GetSeedPath(string savePath) {
			return Path.Combine(savePath, "seed.txt");
		}

		public void DeleteWorld(string world) {
			this.serializationService.Delete(world);
		}

		public static int GetRandomSeed() {
			return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		}
	}
}
