using SoulboundEngine.Core;
using SoulboundEngine.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Client.World.Serialization {
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

		public bool Load(string world, out WorldDump? dump) {
			byte[]? data = saveStrategy.LoadRaw(world);
			if (data == null) {
				dump = null;
				return false;
			}
			dump = pipeline.Read(data);
			return true;
		}

		public string GetSavesRoot() {
			return saveStrategy.GetSavesRoot();
		}
	}
}
