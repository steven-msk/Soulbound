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
            File.WriteAllText(GetDumpPath(name), json);
        }

		public string GetDumpPath(string world) {
			string worldFolder = Path.Combine(root, world);
			Directory.CreateDirectory(GetDataPath(worldFolder));

			string dumpPath = GetDataPath(worldFolder, LevelManager.worldDump);
			return dumpPath;
		}

		public string GetDataPath(params string[] paths) {
			List<string> path = new() { dataPath };
			path.AddRange(paths);
			return Path.Combine(path.ToArray());
		}
	}
}
