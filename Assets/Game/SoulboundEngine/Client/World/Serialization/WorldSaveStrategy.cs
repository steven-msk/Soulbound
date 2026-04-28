using Newtonsoft.Json;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable

namespace SoulboundEngine.Client.World.Serialization {
	public class WorldSaveStrategy : IWorldSaveStrategy {
		private readonly string root;
		private readonly string dataPath;

		public WorldSaveStrategy(string root, string dataPath) {
			this.root = root;
			this.dataPath = dataPath;
		}

		public WorldDump? Load(string name) {
			string path = this.GetDumpPath(name);

			if (File.Exists(path)) {
				return JsonConvert.DeserializeObject<WorldDump>(
					File.ReadAllText(path),
					Soulbound.globalJsonSettings
				);
			}
			return default;
		}

		public void Save(WorldDump obj, string name) {
			string json = JsonConvert.SerializeObject(obj, Soulbound.globalJsonSettings);
			string dumpPath = this.GetDumpPath(name);
			if (string.IsNullOrEmpty(dumpPath)) {
				throw new ArgumentException("Failed to save world: " + name);
			}
			File.WriteAllText(dumpPath, json);
		}

		public void Delete(string world) {

			// TODO: fix unity leak in world saves
			UnityEngine.Windows.Directory.Delete(this.GetSaveFolder(world));
		}

		public byte[]? LoadRaw(string world) {
			string path = this.GetDumpPath(world);
			if (!File.Exists(path)) {
				return null;
			}

			return File.ReadAllBytes(path);
		}

		public void SaveRaw(byte[] data, string world) {
			Directory.CreateDirectory(this.GetSaveFolder(world));
			File.WriteAllBytes(this.GetDumpPath(world), data);
		}

		public string GetDumpPath(string world) {
			string relativeFolder = Path.Combine(this.root, world);
			return this.CombineDataPath(relativeFolder, LevelManager.worldDump);
		}

		public string GetSaveFolder(string world) {
			string relativeFolder = Path.Combine(this.root, world);
			return this.CombineDataPath(relativeFolder);
		}

		private string CombineDataPath(params string[] paths) {
			List<string> path = new() { this.dataPath };
			path.AddRange(paths);
			return Path.Combine(path.ToArray());
		}

		public string GetSavesRoot() => this.CombineDataPath(this.root);
	}
}
