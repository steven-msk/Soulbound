using SoulboundEngine.Common.Math;
using SoulboundEngine.World.Block.State;

namespace SoulboundEngine.World.Chunk {
	using Level = Level.Level;

	public sealed class ChunkSection {
		public const int HEIGHT = 32;
		public const int WIDTH = Level.CHUNK_LENGTH;
		private readonly BlockStateContainer blockStates;

		private ChunkSection(ChunkSection original)
			: this(original.blockStates.Copy()) {
		}

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

		public BlockStateContainer GetStatesImmutable() => this.blockStates.Copy();

		public static SectionPos ComputeLocalPos(int x, int y) {
			int sectionX = Maths.FloorDiv(x, WIDTH);
			int sectionY = Maths.FloorDiv(y, HEIGHT);
			int localX = x - sectionX * WIDTH;
			int localY = y - sectionY * HEIGHT;
			return new SectionPos(localX, localY);
		}

		public ChunkSection Copy() => new(this);
	}
}
