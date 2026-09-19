namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Gen.Generator;
	using SoulboundEngine.World.Level;

#nullable enable

	public class PlacementContext : WorldGenContext {
		public IWorldGenLevel level { get; }
		public ChunkGenerator generator { get; }
		public PlacedFeature? topFeature { get; }

		public PlacementContext(IWorldGenLevel level, ChunkGenerator generator, PlacedFeature? topFeature)
			: base(generator, level) {
			this.level = level;
			this.generator = generator;
			this.topFeature = topFeature;
		}

		public int GetHeight(int blockX) => this.level.GetHeight(blockX);

		public BlockState GetBlockState(BlockPos blockPos) => this.level.GetBlockState(blockPos);

		public int GetMinY() => this.level.GetBottomY();
	}
}
