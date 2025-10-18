using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core {
    [Serializable]
    public struct GameConfig {
        public FileConfig file;
        public DevConfig dev;

        public GameConfig(FileConfig file, DevConfig dev) {
            this.file = file;
            this.dev = dev;
        }
    }

    [Serializable]
    public struct FileConfig {
        public string savesFolder;

        public FileConfig(string savesFolder) {
            this.savesFolder = savesFolder;
        }
    }

    [Serializable]
    public struct DevConfig {
        public bool loadDevWorldFromSave;
        public string devWorld;
        public string devScene;

        public DevConfig(bool loadDevWorldFromSave, string devWorld, string devScene) {
            this.loadDevWorldFromSave = loadDevWorldFromSave;
            this.devWorld = devWorld;
            this.devScene = devScene;
        }
    }
}
