using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;

namespace SoulboundEngine.Client.World.Chunk {
	using Level = Level.Level;

	public class EmptyWorldChunk : WorldChunk {
		public EmptyWorldChunk(Level level, ChunkPos chunkPos) 
			: base(level, chunkPos) {
		}

		public override BlockState GetBlockState(BlockPos blockPos) {
			return Blocks.AIR.DefaultState;
		}

		public override BlockState SetBlockState(BlockPos blockPos, BlockState newState) {
			return null;
		}

		public override void SetTileEntity(TileEntity tileEntity) {
		}

		public override void RemoveTileEntity(BlockPos blockPos) {
		}

		public override TileEntity GetTileEntity(BlockPos blockPos) {
			return null;
		}

		public override bool IsEmpty() => true;
	}
}
