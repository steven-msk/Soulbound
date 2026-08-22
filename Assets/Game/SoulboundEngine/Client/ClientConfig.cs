namespace SoulboundEngine.Client {
	using System;
	using UnityEngine;

	[Serializable]
    public struct ClientConfig {
        public File file;
        public Dev dev;
		public Unity unity;
		[HideInInspector] public bool isRunningInEditor;

		[Serializable]
		public struct File {
			public string savesRoot;
			public string seedFile;
			public string chunksFolder;
		}

		[Serializable]
		public struct Dev {
			public bool overrideSaves;
			public string devWorld;
			public int seed;
		}


		[Serializable]
		public struct Unity {
			public string mainScene;
			public string worldScene;
		}
	}
}
