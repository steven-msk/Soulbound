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
        public WorldDump Load(string path) {
            if (File.Exists(path)) {
                return JsonConvert.DeserializeObject<WorldDump>(
                    File.ReadAllText(path),
                    LevelManager.globalJsonSettings
                );
            }
            return default;
        }

        public void Save(WorldDump obj, string path) {
            string json = JsonConvert.SerializeObject(obj, LevelManager.globalJsonSettings);
            File.WriteAllText(path, json);
        }
    }
}
