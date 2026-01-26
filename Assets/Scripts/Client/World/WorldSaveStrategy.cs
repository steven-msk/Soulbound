using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.World {
	public class WorldSaveStrategy : IWorldSaveStrategy {
		private readonly string root;
		private readonly string dataPath;

		public WorldSaveStrategy(string root, string dataPath) {
			this.root = root;
			this.dataPath = dataPath;
		}

		public WorldDump? Load(string name) {
			string path = GetDumpPath(name);

			if (File.Exists(path)) {
				return JsonConvert.DeserializeObject<WorldDump>(
					File.ReadAllText(path),
					LevelManager.globalJsonSettings
				);
			}
			return default;
		}

		public void Save(WorldDump obj, string name) {
			string json = JsonConvert.SerializeObject(obj, LevelManager.globalJsonSettings);
			string dumpPath = GetDumpPath(name);
			if (string.IsNullOrEmpty(dumpPath)) {
				throw new ArgumentException("Failed to save world: " + name);
			}
			File.WriteAllText(dumpPath, json);
		}

		public byte[]? LoadRaw(string world) {
			string path = GetDumpPath(world);
			if (!File.Exists(path)) {
				return null;
			}

			return File.ReadAllBytes(path);
		}

		public void SaveRaw(byte[] data, string world) {
			Directory.CreateDirectory(GetSaveFolder(world));
			File.WriteAllBytes(GetDumpPath(world), data);
		}

		public string GetDumpPath(string world) {
			string relativeFolder = Path.Combine(root, world);
			return CombineDataPath(relativeFolder, LevelManager.worldDump);
		}

		public string GetSaveFolder(string world) {
			string relativeFolder = Path.Combine(root, world);
			return CombineDataPath(relativeFolder);
		}

		private string CombineDataPath(params string[] paths) {
			List<string> path = new() { dataPath };
			path.AddRange(paths);
			return Path.Combine(path.ToArray());
		}

		public string GetSavesRoot() => CombineDataPath(root);
	}
}
