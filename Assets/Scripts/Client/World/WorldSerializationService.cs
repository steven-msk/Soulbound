using SoulboundBackend.Core;
using SoulboundBackend.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World {
	public sealed class WorldSerializationService {
		private readonly IWorldSaveStrategy saveStrategy;
		private readonly ISerializationPipeline<WorldDump> pipeline;

		public WorldSerializationService(IWorldSaveStrategy saveStrategy, ISerializationPipeline<WorldDump> pipeline) {
			this.saveStrategy = saveStrategy;
			this.pipeline = pipeline;
		}

		public void Save(WorldDump dump, string world) {
			byte[] data = pipeline.Write(dump);
			saveStrategy.SaveRaw(data, world);
		}

		public WorldDump? Load(string world) {
			byte[]? data = saveStrategy.LoadRaw(world);
			if (data == null) {
				return null;
			}
			return pipeline.Read(data);
		}

		public string GetSavesRoot() {
			UnityEngine.Debug.Log(saveStrategy.GetType());
			return saveStrategy.GetSavesRoot();
		}
	}
}
