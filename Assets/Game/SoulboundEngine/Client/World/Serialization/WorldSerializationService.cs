using SoulboundEngine.Core.Serialization;

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
			byte[] data = this.pipeline.Write(dump);
			this.saveStrategy.SaveRaw(data, world);
		}

		public bool Load(string world, out WorldDump? dump) {
			byte[]? data = this.saveStrategy.LoadRaw(world);
			if (data == null) {
				dump = null;
				return false;
			}
			dump = this.pipeline.Read(data);
			return true;
		}

		public string GetSavesRoot() {
			return this.saveStrategy.GetSavesRoot();
		}

		public void Delete(string world) {
			this.saveStrategy.Delete(world);
		}
	}
}
