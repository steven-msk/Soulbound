using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Core {
    [Serializable]
    public struct GameConfig {
        public FileConfig file;
        public DevConfig dev;
    }

    [Serializable]
    public struct FileConfig {
        public string savesFolder;
    }

    [Serializable]
    public struct DevConfig {
		public bool useDoNotSaveWorldStrategy;
		public string devWorld;
		public string devScene;
		public int seed;
    }
}
