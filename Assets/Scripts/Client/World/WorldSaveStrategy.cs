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
    public class WorldSaveStrategy : ISaveStrategy<WorldDump> {
        private readonly string root;
        private readonly string dataRegion;

        public WorldSaveStrategy(string root, string dataRegion) {
            this.root = root;
            this.dataRegion = dataRegion;
        }

        public WorldDump Load(string name) {
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
			Directory.CreateDirectory(GetRegionedPath(worldFolder));

			string dumpPath = GetRegionedPath(Path.Combine(worldFolder, LevelManager.worldDump));
			return dumpPath;
		}

		public string GetRegionedPath(params string[] paths) {
			List<string> regioned = new() { dataRegion };
			regioned.AddRange(paths);
			return Path.Combine(regioned.ToArray());
		}
	}
}
