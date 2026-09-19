namespace SoulboundEngine.World.Level {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;

#nullable enable

	public interface ILevelView : IBlockView {
		Chunk? GetChunk(int chunkX, bool loadOrGenerate);

		int GetHeight(int blockX);

		RegistryEntry<Biome> GetBiome(BlockPos blockPos);

		LevelType GetLevelType();

		IRegistryManager GetRegistries();

		int IHeightLimitView.GetBottomY() => this.GetLevelType().minY;

		int IHeightLimitView.GetHeight() => this.GetLevelType().Height;

		public virtual BlockPos GetTopPosition(BlockPos blockPos) {
			return new BlockPos(blockPos.x, this.GetHeight(blockPos.y));
		}
	}

	public static class ReadableLevelDefaults {
		public static int GetHeight(this ILevelView level, BlockPos blockPos) {
			return level.GetHeight(blockPos.x);
		}

		
	}
}
