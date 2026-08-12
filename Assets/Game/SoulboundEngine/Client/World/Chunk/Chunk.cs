using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World.Chunk {
	public abstract class Chunk : IHeightLimitView {
		protected readonly Dictionary<BlockPos, TileEntity> tileEntities = new();
		protected readonly IHeightLimitView heightLimitView;
		protected readonly ChunkSection[] sections;
		protected readonly ChunkPos chunkPos;

		public Chunk(ChunkPos chunkPos, ChunkSection[]? sections, IHeightLimitView heightLimitView, Func<BlockStateContainer> containerFactory) {
			this.chunkPos = chunkPos;
			this.sections = new ChunkSection[heightLimitView.GetSectionCount()];
			this.heightLimitView = heightLimitView;
			if (sections != null) {
				if (sections.Length == this.sections.Length) {
					Array.Copy(sections, this.sections, sections.Length);
				} else {
					Logger.LogWarning("Could not set chunk sections, array length is {} instead of {}",
						sections.Length, this.sections.Length);
				}
			}
			ReplaceMissingSections(containerFactory, this.sections);
		}

		private static void ReplaceMissingSections(Func<BlockStateContainer> containerFactory, ChunkSection[] sections) {
			for (int i = 0; i < sections.Length; i++) {
				if (sections[i] == null) {
					sections[i] = new ChunkSection(containerFactory());
				}
			}
		}
		
		public abstract BlockState? SetBlockState(BlockPos blockPos, BlockState state);

		public abstract void SetTileEntity(TileEntity tileEntity);

		public abstract void RemoveTileEntity(BlockPos blockPos);

		public int GetBottomY() => this.heightLimitView.GetBottomY();

		public int GetHeight() => this.heightLimitView.GetHeight();

		public HashSet<BlockPos> GetTileEntityPositions() {
			return this.tileEntities.Keys.ToHashSet();
		}

		public ChunkSection[] GetSections() => this.sections;
		public ChunkSection GetSection(int yIndex) => this.sections[yIndex];
	}
}
