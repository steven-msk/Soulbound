using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World.Chunk {
	public abstract class Chunk : IBlockGetter {
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

		public abstract void Tick();
		
		public abstract BlockState? SetBlockState(BlockPos blockPos, BlockState state);
		public abstract BlockState GetBlockState(BlockPos blockPos);


		public abstract void SetTileEntity(TileEntity tileEntity);
		public abstract TileEntity? GetTileEntity(BlockPos blockPos);
		public abstract void RemoveTileEntity(BlockPos blockPos);

		public abstract JObject? GetTileEntityJsonForSaving(BlockPos blockPos);

		public int GetBottomY() => this.heightLimitView.GetBottomY();

		public int GetHeight() => this.heightLimitView.GetHeight();

		public HashSet<BlockPos> GetTileEntityPositions() {
			return this.tileEntities.Keys.ToHashSet();
		}

		public virtual bool CanBeSerialized() => true;

		public ChunkPos GetPos() => this.chunkPos;

		public abstract bool IsEmpty();

		public ChunkSection[] GetSections() => this.sections;
		public ChunkSection GetSection(int yIndex) => this.sections[yIndex];
	}
}
