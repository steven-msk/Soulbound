using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public static class StateFileHandler {
		private static readonly string cacheDir = Path.Combine(Application.persistentDataPath, "blockStates");

		public static IEnumerable<BlockState> LoadAll(Block block) {
			string path = GetFilePath(block);
			if (!File.Exists(path)) {
				return Enumerable.Empty<BlockState>();
			}

			var entries = Deserialize<List<BlockState.PersistencyInfo>>(path);
			return entries?.Select(data => data.ToBlockState(block)) 
				?? Enumerable.Empty<BlockState>();
		}

		public static BlockState? LoadSingle(Block block, int hash) {
			string path = GetFilePath(block);
			if (!File.Exists(path)) {
				return null;
			}

			var entries = Deserialize<List<BlockState.PersistencyInfo>>(path);
			return SelectPersistencyInfo(entries, hash)?.ToBlockState(block);
		}

		public static BlockState.PersistencyInfo? GetPersistencyInfo(Block block, int hash) {
			string path = GetFilePath(block);
            if (!File.Exists(path)) {
				return null;
            }

			var entries = Deserialize<List<BlockState.PersistencyInfo>>(path);
			return SelectPersistencyInfo(entries, hash);
        }

		private static BlockState.PersistencyInfo? SelectPersistencyInfo(List<BlockState.PersistencyInfo>? deserialized, int hash) {
			return deserialized?.FirstOrDefault(e => e.hash == hash);
		}

		public static void Save(Block block, IEnumerable<BlockState> states) {
			Directory.CreateDirectory(cacheDir); 
			string path = GetFilePath(block);

			var serialized = JsonConvert.SerializeObject(
				states.Select(BlockState.PersistencyInfo.From).ToList(), 
				LevelManager.globalJsonSettings
			);

			File.WriteAllText(path, serialized);
		}

		private static string GetFilePath(Block block) {
			return Path.Combine(cacheDir, $"{block.hashedID}_.json");
		}

		private static T? Deserialize<T>(string path) {
			return JsonConvert.DeserializeObject<T>(
				File.ReadAllText(path),
				LevelManager.globalJsonSettings
			);
		}
	}
}
