using Newtonsoft.Json.Linq;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using System;
using System.Collections.Generic;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.World.Chunk {
	using Block = Block.Block;
	using Level = Level.Level;

	public class WorldChunk : Chunk {
		public const float HEIGHT_SPREAD = 0.01f;
		public const float SURFACE_HEIGHT_RANGE = 50f;
		public const float UNDERGROUND_HEIGHT_RANGE = 20f;
		private readonly TileEntityTickManager tickManager = new();
		private readonly Level level;
		public int ChunkX => this.chunkPos.x;

		public WorldChunk(Level level, ChunkPos chunkPos) 
			: this(level, chunkPos, null, level.BlockStateContainerFactory()) { 
			this.level = level;
		}

		public WorldChunk(Level level, ChunkPos chunkPos, ChunkSection[]? sections, Func<BlockStateContainer> containerFactory)
			: base(chunkPos, sections, level, containerFactory) {
			this.level = level;
		}

		public int[]? surfacePoints { get; set; }

		public override void Tick() => this.tickManager.Tick();

		public static int WorldYToIndex(int worldY) => worldY - Level.MIN_Y;

		public static int IndexToWorldY(int yIndex) => yIndex + Level.MIN_Y;

		public int WorldXToChunkX(int x) => x - this.ChunkX * Level.CHUNK_LENGTH;

		public int ChunkXToWorldX(int cx) => cx + this.ChunkX * Level.CHUNK_LENGTH;

		public override BlockState? SetBlockState(BlockPos blockPos, BlockState newState) {
			ChunkSection section = this.GetSection(this.GetSectionIndexFromBlock(blockPos.y));
			bool wasEmpty = section.HasOnlyAir;
			if (wasEmpty && newState.IsAir()) return null;

			SectionPos sectionPos = ChunkSection.ComputeLocalPos(blockPos.x, blockPos.y);
			BlockState oldState = section.SetBlockState(sectionPos.x, sectionPos.y, newState);
			if (oldState == newState) return null;

			Block newBlock = newState.GetBlock();
			bool blockChanged = !oldState.IsOf(newBlock);
			if (blockChanged && oldState.HasTileEntity() && !newState.ShouldChangedStateKeepTileEntity(oldState)) {
				this.RemoveTileEntity(blockPos);
			}

			if (!section.GetBlockState(sectionPos.x, sectionPos.y).IsOf(newBlock)) return null;
			newState.OnPlace(this.level, blockPos, oldState);

			if (newState.HasTileEntity()) {
				TileEntity? currentEntity = this.GetTileEntity(blockPos);
				if (currentEntity != null && !currentEntity.IsValidBlockState(newState)) {
					Logger.LogWarning("Found mismatched tile entity at {}: type = {}, state = {}",
						blockPos, currentEntity.GetTileEntityType(), newState);
					this.RemoveTileEntity(blockPos);
					currentEntity = null;
				}

				if (currentEntity == null) {
					currentEntity = ((ITileEntityProvider)newBlock).CreateTileEntity(blockPos, newState);
					if (currentEntity != null) this.SetTileEntity(currentEntity);
				} else {
					currentEntity.SetBlockState(newState);
				}
			}

			return oldState;
		}

		[Obsolete]
		public void SetBlock(ChunkBlockPos chunkPos, BlockState blockState) {
			this.SetBlock(chunkPos.x, WorldYToIndex(chunkPos.y), blockState);
		}
		[Obsolete]
		public void SetBlock(int cx, int yIndex, BlockState blockState) {
			this.SetBlockState(new BlockPos(this.ChunkXToWorldX(cx), IndexToWorldY(yIndex)), blockState);
		}

		public BlockState GetBlockState(ChunkBlockPos chunkPos) => this.GetBlockState(chunkPos.ToBlock());

		public override BlockState GetBlockState(BlockPos blockPos) {
			int sectionIndex = this.GetSectionIndexFromBlock(blockPos.y);
			if (sectionIndex < 0 || sectionIndex >= this.sections.Length) return Blocks.AIR.DefaultState;

			ChunkSection section = this.GetSection(sectionIndex);
			if (section.HasOnlyAir) return Blocks.AIR.DefaultState;

			SectionPos sectionPos = ChunkSection.ComputeLocalPos(blockPos.x, blockPos.y);
			return section.GetBlockState(sectionPos.x, sectionPos.y);
		}

		public override TileEntity? GetTileEntity(BlockPos blockPos) {
			return this.tileEntities.TryGetValue(blockPos, out TileEntity tileEntity)
				? tileEntity
				: null;
		}

		public override void SetTileEntity(TileEntity tileEntity) {
			BlockPos blockPos = tileEntity.blockPos;
			BlockState state = this.GetBlockState(blockPos);
			BlockState cachedState = tileEntity.GetBlockState();
			bool mismatchedState = state != cachedState && !tileEntity.GetTileEntityType().Supports(state);
			if (!state.HasTileEntity() || mismatchedState) {
				Logger.LogWarning("Trying to set tile entity {} at {}, but state {} does not allow it", tileEntity, blockPos, state);
				return;
			}

			if (mismatchedState) {
				if (state.GetBlock() != cachedState.GetBlock()) {
					Logger.LogWarning("Block state mismatch on tile entity {} at {}, updating", tileEntity, blockPos);
				}
				tileEntity.SetBlockState(state);
			}

			if (this.tileEntities.TryGetValue(blockPos, out TileEntity previousEntity) && previousEntity != tileEntity) {
				this.tickManager.RemoveTileEntity(previousEntity);
				tileEntity.OnDispose();
			}
			tileEntity.SetLevel(this.level);
			this.tickManager.AddTileEntity(tileEntity);
			this.tileEntities[blockPos] = tileEntity;
		}

		public override void RemoveTileEntity(BlockPos blockPos) {
			TileEntity? tileEntity = this.GetTileEntity(blockPos);
			if (tileEntity == null) return;

			this.tickManager.RemoveTileEntity(tileEntity);
			this.tileEntities.Remove(blockPos);
			tileEntity.OnDispose();
		}

		public override JObject? GetTileEntityJsonForSaving(BlockPos blockPos) {
			TileEntity? tileEntity = this.GetTileEntity(blockPos);
			if (tileEntity == null) return null;

			JObject json = new() {
				["type"] = TileEntityType.GetId(tileEntity.GetTileEntityType())!.ToString(),
				["pos"] = tileEntity.blockPos.ToString(),
			};
			tileEntity.Write(json);
			return json;
		}

		public override bool IsEmpty() => false;

		public IEnumerable<TileEntity> GetTileEntities() => this.tileEntities.Values;
	}
}
