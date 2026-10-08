namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.World;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using System;

#nullable enable

	public sealed class EditorWorldGenChunk : Chunk {
		public EditorWorldGenChunk(ChunkPos chunkPos, IHeightLimitView heightLimitView, Func<BlockStateContainer> stateContainerFactory) 
			: base(chunkPos, null, heightLimitView, stateContainerFactory) {
		}

		public override BlockState GetBlockState(BlockPos blockPos) {
			int sectionIndex = this.GetSectionIndexFromBlock(blockPos.y);
			if (sectionIndex < 0 || sectionIndex >= this.sections.Length) return Blocks.AIR.DefaultState;

			ChunkSection section = this.GetSection(sectionIndex);
			if (section.HasOnlyAir) return Blocks.AIR.DefaultState;

			SectionPos sectionPos = ChunkSection.ComputeLocalPos(blockPos.x, blockPos.y);
			return section.GetBlockState(sectionPos.x, sectionPos.y);
		}

		public override BlockState? SetBlockState(BlockPos blockPos, BlockState state) {
			ChunkSection section = this.GetSection(this.GetSectionIndexFromBlock(blockPos.y));
			bool wasEmpty = section.HasOnlyAir;
			if (wasEmpty && state.IsAir()) return null;

			SectionPos sectionPos = ChunkSection.ComputeLocalPos(blockPos.x, blockPos.y);
			BlockState oldState = section.SetBlockState(sectionPos.x, sectionPos.y, state);
			if (oldState == state) return null;

			Block newBlock = state.GetBlock();
			return !section.GetBlockState(sectionPos.x, sectionPos.y).IsOf(newBlock) ? null : oldState;
		}

		public override void Tick() {
		}

		public override TileEntity? GetTileEntity(BlockPos blockPos) {
			return null;
		}

		public override JToken GetTileEntityJsonForSaving(BlockPos blockPos) {
			return JValue.CreateNull();
		}

		public override bool IsEmpty() => false;

		public override void SetTileEntity(TileEntity tileEntity) {
		}

		public override void RemoveTileEntity(BlockPos blockPos) {
		}

	}
}
