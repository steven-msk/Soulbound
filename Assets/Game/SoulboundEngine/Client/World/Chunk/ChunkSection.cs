using SoulboundEngine.Client.World.Block.State;

namespace SoulboundEngine.Client.World.Chunk {
	using Level = Level.Level;

	public sealed class ChunkSection {
		public const int HEIGHT = 32;
		public const int WIDTH = Level.CHUNK_LENGTH;
		private readonly BlockStateContainer blockStates;

		public ChunkSection(BlockStateContainer blockStates) {
			this.blockStates = blockStates;
		}

		public BlockState GetBlockState(int x, int y) {
			return this.blockStates.Get(x, y);
		}

		public BlockState SetBlockState(int x, int y, BlockState state) {
			BlockState previous = this.GetBlockState(x, y);
			this.blockStates.Set(x, y, state);
			return previous;
		}

		public bool HasOnlyAir => this.blockStates.HasOnlyAir;

		public static ChunkSectionPos ComputeLocalPos(int x, int y) {
			int sectionX = x / WIDTH;
			int sectionY = y / HEIGHT;
			int localX = x - sectionX * WIDTH;
			int localY = y - sectionY * HEIGHT;
			return new ChunkSectionPos(localX, localY, sectionX, sectionY);
		}
	}
}
