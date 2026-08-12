using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Generation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.World.Chunk {
	using Block = Block.Block;
	using Level = Level.Level;

	public class WorldChunk : Chunk {
		public const float HEIGHT_SPREAD = 0.01f;
		public const float SURFACE_HEIGHT_RANGE = 50f;
		public const float UNDERGROUND_HEIGHT_RANGE = 20f;
		private readonly TileEntityTickManager tickManager = new();
		private readonly Level level;
		public int chunkX => this.chunkPos.x;

		public WorldChunk(Level level, ChunkPos chunkPos) 
			: this(level, chunkPos, null, level.BlockStateContainerFactory()) { 
			this.level = level;
		}

		public WorldChunk(Level level, ChunkPos chunkPos, ChunkSection[]? sections, Func<BlockStateContainer> containerFactory)
			: base(chunkPos, sections, level, containerFactory) {
			this.level = level;
		}

		public static void CreateBlockArray(ref int[][] array) {
			array = new int[Level.CHUNK_LENGTH][];
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				array[x] = new int[Level.WORLD_HEIGHT];
			}
		}

		public void Tick() => this.tickManager.Tick();

		[Obsolete("known issue: world architecture design is poorly designed")]
		public void Generate(BiomeMap biomeMap, Heightmap heightmap, Cavemap cavemap, bool placeBlocks, out ChunkGenData genData) {
			genData = new ChunkGenData {
				chunk = this,
				genContexts = new BlockGenContext[Level.CHUNK_LENGTH][],
				surfacePoints = new int[Level.CHUNK_LENGTH],
				biomeWeights = new IEnumerable<BiomeWeight>[Level.CHUNK_LENGTH],
				biomePartition = new ChunkBiomePartition(),
				caveDensities = new float[Level.CHUNK_LENGTH][],
				caveMask = new BitArray[Level.CHUNK_LENGTH]
			};

			for (int cx = 0; cx < Level.CHUNK_LENGTH; cx++) {
				genData.caveDensities[cx] = new float[Level.WORLD_HEIGHT];
				genData.caveMask[cx] = new BitArray(Level.WORLD_HEIGHT);
				genData.genContexts[cx] = new BlockGenContext[Level.WORLD_HEIGHT];
				int x = this.ChunkXToWorldX(cx);

				var weights = biomeMap.ResolveWeights(x);
				biomeMap.ResolvePrimaryBiomes(weights, out var primary, out var secondary);
				genData.biomeWeights[cx] = weights;

				var partition = this.ProcessBiomePartition(x, primary.biome, genData.biomePartition);
				genData.biomePartition = partition;

				int height = Mathf.FloorToInt(heightmap.SampleHeight(x, primary, secondary));
				float surfaceY = heightmap.ToYCoord(height);

				BlockResolver blockResolver = new(primary.biome, secondary?.biome);

				for (int y = 0; y < Level.WORLD_HEIGHT; y++) {
					BlockPos blockPos = new(x, IndexToWorldY(y));
					float caveDensity = cavemap.SampleDensity(x, blockPos.y, surfaceY, primary, secondary);
					bool isCave = cavemap.IsCave(caveDensity);

					var ctx = new BlockGenContext {
						pos = blockPos,
						surfaceY = heightmap.ToYCoord(height),
						caveDensity = caveDensity,
						isCave = isCave,
					};

					genData.genContexts[cx][y] = ctx;
					genData.caveDensities[cx][y] = caveDensity;
					genData.caveMask[cx][y] = isCave;
					genData.surfacePoints[cx] = ctx.surfaceY;

					if (placeBlocks) {
						BlockState blockState = blockResolver.ResolveBlock(ctx);
						this.SetBlock(cx, y, blockState);
					}
				}
			}
		}

		[Obsolete]
		ChunkBiomePartition ProcessBiomePartition(int x, IBiome primary, ChunkBiomePartition partition) {
			if (partition.primary == null) {
				partition.primary = primary;
			}
			
			if (partition.primary != primary && partition.secondary == null) {
				partition.secondary = primary;
				partition.splitX = x;
			}
			return partition;
		}

		[Obsolete]
		public void PostProcess(ChunkGenData genData, Level level) {
			IBiome primary = genData.biomePartition.primary;
			IBiome? secondary = genData.biomePartition.secondary;

			int splitX = genData.biomePartition.splitX;
			int chunkStartX = this.ChunkXToWorldX(0);
			int chunkEndX = this.ChunkXToWorldX(Level.CHUNK_LENGTH - 1);

			int partitionStartX = chunkStartX;
			int partitionLimitX = secondary == null ? chunkEndX : splitX;
			primary.PostProcess(genData, this, level, partitionStartX, partitionLimitX);

			if (secondary != null) {
				partitionStartX = splitX + 1;
				partitionLimitX = chunkEndX;

				secondary?.PostProcess(genData, this, level, partitionStartX, partitionLimitX);
			}
		}

		public static int WorldYToIndex(int worldY) => worldY - Level.MIN_Y;

		public static int IndexToWorldY(int yIndex) => yIndex + Level.MIN_Y;

		public int WorldXToChunkX(int x) => x - this.chunkX * Level.CHUNK_LENGTH;

		public int ChunkXToWorldX(int cx) => cx + this.chunkX * Level.CHUNK_LENGTH;

		public override BlockState? SetBlockState(BlockPos blockPos, BlockState newState) {
			ChunkSection section = this.GetSection(this.GetSectionIndexFromBlock(blockPos.y));
			bool wasEmpty = section.HasOnlyAir;
			if (wasEmpty && newState.IsAir()) return null;

			ChunkSectionPos sectionPos = ChunkSection.ComputeLocalPos(blockPos.x, blockPos.y);
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

		public void SetAllBlocks(int[][] stateIDs) {
			Array.Copy(stateIDs, this.blockStateIDs, stateIDs.Length);
		}

		/// <summary>
		/// Trust boundary on deserialized input. 
		/// This checks whether the tileEntity claims to belong to a block that matches the expected outcome.
		/// </summary>
		public bool ValidateTileEntity(TileEntity tileEntity) {
			ChunkBlockPos chunkPos = tileEntity.blockPos.ToChunkPos();
			BlockState? stateInChunk = this.GetBlockState(chunkPos);

			if (tileEntity.GetBlockState() != stateInChunk) {
				if (stateInChunk != null) {
					Logger.LogError("Block state in tile entity does not match the one in chunk: {} at {}, expected {} but was {}",
						tileEntity, chunkPos, tileEntity.GetBlockState(), stateInChunk!);
				} else {
					Logger.LogError("Deserialized TileEntity {} at {} is out of world bounds", tileEntity, chunkPos);
				}
				return false;
			}
			if (stateInChunk.block is not ITileEntityProvider) {
				Logger.LogError("Block state in chunk is not of type ITileEntityProvider, " +
					"but a TileEntity was associated with it: {} at {}, block in chunk: {}",
					tileEntity, chunkPos, stateInChunk);
				return false;
			}
			return true;
		}

		public void AddTileEntityValidated(TileEntity tileEntity) {
			if (this.tileEntities.TryGetValue(tileEntity.blockPos, out TileEntity existing)) {
				Logger.LogWarning("Validated TileEntity already exists: attempted to add {} at {} but {} was already there",
					tileEntity, tileEntity.blockPos, existing);
				return;
			}
			this.tileEntities.Add(tileEntity.blockPos, tileEntity);
		}

		/// <summary>
		/// Consistency check after deserialized tile entities are applied and validated.
		/// This makes sure every provider block is backed by a tile entity.
		/// </summary>
		public void SyncBlocksWithTileEntities() {
			for (int x = 0; x < this.blockStateIDs.Length; x++) {
				for (int y = 0; y < this.blockStateIDs[x].Length; y++) {
					BlockState blockState = Block.GetState(this.blockStateIDs[x][y]);
					BlockPos blockPos = new(x, IndexToWorldY(y));

					TileEntity? tileEntityAtBlock = this.GetTileEntity(blockPos);
					if (blockState.block is ITileEntityProvider tileEntityProvider && tileEntityAtBlock == null) {
						// a TileEntity rejected by ValidateTileEntity should not vanish silently
						// instead, a fresh TileEntity is created from the provider
						Logger.LogWarning("Found missing tile entity for block {} at {}. This may be the result of broken serialization data",
							Blocks.GetIdentifier(blockState.block), blockPos);

						TileEntity? tileEntity = tileEntityProvider.CreateTileEntity(blockPos, blockState);
						if (tileEntity != null && tileEntity.GetTileEntityType().Supports(blockState)) {
							tileEntity.SetLevel(this.level);
							this.tileEntities[blockPos] = tileEntity;
							this.tickManager.AddTileEntity(tileEntity);
							Logger.LogInfo("Added fresh TileEntity since it was missing: {} at {}", tileEntity, blockPos);
						} else {
							Logger.LogError("Failed to replenish missing TileEntity for block {} at {}",
								Blocks.GetIdentifier(blockState.block), blockPos);
							this.tileEntities.Remove(blockPos);
						}
					} else if (blockState.block is not ITileEntityProvider && tileEntityAtBlock != null) {
						// branch kept for consistency with ValidateTileEntity and defense against other code paths
						// to avoid stale tile entities remaining in the dictionary.
						// should be unreachable given a pass to ValidateTileEntity before calling this
						Logger.LogWarning("Found TileEntity for block that doesnt provide tile entities: {} at {}", tileEntityAtBlock, blockPos);
						this.tileEntities.Remove(blockPos);
					} else if (tileEntityAtBlock != null) {
						tileEntityAtBlock.SetLevel(this.level);
						this.tickManager.AddTileEntity(tileEntityAtBlock);
					}
				}
			}
		}

		public BlockState GetBlockState(ChunkBlockPos chunkPos) => this.GetBlockState(chunkPos.ToBlock());

		public BlockState GetBlockState(BlockPos blockPos) {
			int sectionIndex = this.GetSectionIndexFromBlock(blockPos.y);
			if (sectionIndex < 0 || sectionIndex >= this.sections.Length) return Blocks.AIR.DefaultState;

			ChunkSection section = this.GetSection(sectionIndex);
			if (section.HasOnlyAir) return Blocks.AIR.DefaultState;

			ChunkSectionPos sectionPos = ChunkSection.ComputeLocalPos(blockPos.x, blockPos.y);
			return section.GetBlockState(sectionPos.x, sectionPos.y);
		}

		public TileEntity? GetTileEntity(BlockPos blockPos) {
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

		public IEnumerable<TileEntity> GetTileEntities() => this.tileEntities.Values;
	}
}
