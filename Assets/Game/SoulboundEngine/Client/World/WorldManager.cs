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

		public IEnumerable<string> ListSaves() {
			string savesRoot = this.serializationService.GetSavesRoot();
			if (!Directory.Exists(savesRoot)) yield break;

			foreach (var dir in Directory.GetDirectories(savesRoot)) {
				if (File.Exists(Path.Combine(dir, LevelManager.worldDump))) {
					yield return Path.GetFileName(dir);
				}
			}
		}

		public void CreateNewWorld(string world) {
			if (this.ListSaves().Any(s => s == world)) {
				throw new ArgumentException($"World with name '{world}' already exists");
			}

			string savesRoot = this.serializationService.GetSavesRoot();
			string worldPath = Path.Combine(savesRoot, world);
			var dir = Directory.CreateDirectory(worldPath);
			var filePath = Path.Combine(worldPath, LevelManager.worldDump);
			File.WriteAllText(filePath, string.Empty);
		}

		public void DeleteWorld(string world) {
			this.serializationService.Delete(world);
		}
	}
}
