#if UNITY_EDITOR

using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

#nullable enable


namespace SoulboundBackend.Client.World {
	public class DoNotSaveWorldStrategy : IWorldSaveStrategy {
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
