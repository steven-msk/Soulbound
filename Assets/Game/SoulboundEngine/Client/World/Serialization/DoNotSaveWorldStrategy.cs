using System.IO;
using UnityEditor;

#nullable enable

#if UNITY_EDITOR

namespace SoulboundEngine.Client.World.Serialization {
	public class DoNotSaveWorldStrategy : IWorldSaveStrategy {
		public void Delete(string world) {
		}

		string IWorldSaveStrategy.GetSavesRoot() => FileUtil.GetUniqueTempPathInProject();

		WorldDump? IWorldSaveStrategy.Load(string world) => null;

		byte[]? IWorldSaveStrategy.LoadRaw(string world) => null;

		void IWorldSaveStrategy.Save(WorldDump obj, string world) {
			Directory.Delete(Path.Combine(FileUtil.GetUniqueTempPathInProject(), world));
		}

		void IWorldSaveStrategy.SaveRaw(byte[] data, string world) {
		}
	}
}

#endif
