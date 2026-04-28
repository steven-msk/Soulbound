using System;

namespace SoulboundEngine.Core {
    [Serializable]
    public struct GameConfig {
        public FileConfig file;
        public DevConfig dev;
		public UnityConfig unity;
    }

    [Serializable]
    public struct FileConfig {
        public string savesFolder;
    }

    [Serializable]
    public struct DevConfig {
		public bool useDoNotSaveWorldStrategy;
		public string devWorld;
		public int seed;
    }

	[Serializable]
	public struct UnityConfig {
		public string mainScene;
		public string worldScene;
	}
}
