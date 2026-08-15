using SoulboundEngine.Serialization;
using UnityEngine;

namespace SoulboundEngine.Client.IO {
	public static class UnityPaths {
		/// <summary>
		/// Writable, per-user save/config root. <br/>
		/// Windows: <c>%userprofile%/AppData/LocalLow/{company}/{product}</c>
		/// </summary>
		public static File PersistentDataRoot => new(Application.persistentDataPath);

		/// <summary>
		/// Read-only install directory.
		/// </summary>
		public static File DataRoot => new(Application.dataPath);

		/// <summary>
		/// Read-only bundled assets. On some platforms this is
		/// inside a compressed archive and cannot be accessed via System.IO.
		/// Use UnityWebRequest for StreamingAssets reads if targeting Android.
		/// </summary>
		public static File StreamingAssetsRoot => new(Application.streamingAssetsPath);

		/// <summary>Temporary/cache directory, may be cleared by the OS.</summary>
		public static File TemporaryCacheRoot => new(Application.temporaryCachePath);
	}
}