namespace SoulboundEngine.Client {
	using System;
	using UnityEngine;

	[Serializable]
    public struct ClientConfig {
        public FileConfig file;
        public DevConfig dev;
		public UnityConfig unity;
		[HideInInspector] public bool isRunningInEditor;
    }

    [Serializable]
    public struct FileConfig {
        public string savesRoot;
		public string seedFile;
		public string chunksFolder;
    }

    [Serializable]
    public struct DevConfig {
		public bool overrideSaves;
		public string devWorld;
		public int seed;
    }

	[Serializable]
	public struct UnityConfig {
		public string mainScene;
		public string worldScene;
	}
}
