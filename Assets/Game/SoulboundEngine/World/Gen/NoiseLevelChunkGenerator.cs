namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Chunk;
	using System;

#nullable enable

	public sealed class NoiseLevelChunkGenerator : ChunkGenerator {
		// TEMPORARY
		public new static readonly MapCodec<ChunkGenerator> CODEC = RecordMapCodec<ChunkGenerator, int>.Of(
			Field.Required<ChunkGenerator, int>("temp", Codecs.INT, v => 1),
			i => default
		);

		[Obsolete]
		public NoiseLevelChunkGenerator()
			: base(null) {
		}

		protected override MapCodec<ChunkGenerator> Codec() => CODEC;

		public override Chunk Fill(RandomState randomState, Chunk chunk) {
			throw new NotImplementedException();
		}

		public override Chunk BuildSurface(RandomState randomState, Chunk chunk) {
			throw new NotImplementedException();
		}

		public override int GetMinGenY() {
			throw new NotImplementedException();
		}

		public override int GetGenHeight() {
			throw new NotImplementedException();
		}

		public override int GetBaseHeight(int x, IHeightLimitView heightLimit) {
			throw new NotImplementedException();
		}
	}
}
