using Cysharp.Threading.Tasks;
using SoulboundBackend.Client.World;
using SoulboundBackend.Core;
using SoulboundBackend.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Logger = SoulboundBackend.Client.Debug.Logging.Logger;
using SoulboundBackend.Client.World.Serialization;
using SoulboundBackend.Client.World.LevelDomain;

#nullable enable

namespace SoulboundBackend.Client.World {
	public sealed class WorldManager {
		private readonly WorldSerializationService serializationService;

		public WorldManager(WorldSerializationService serializationService) {
			this.serializationService = serializationService;
		}

		public IEnumerable<string> ListSaves() {
			string savesRoot = serializationService.GetSavesRoot();
			if (!Directory.Exists(savesRoot)) yield break;

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

	}
}
