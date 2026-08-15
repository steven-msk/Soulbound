using SoulboundEngine.World.Biome;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.Client.World.Chunk;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.Gen {
	using Chunk = Chunk.Chunk;
	using Level = Level.Level;

	public sealed class NoiseLevelChunkGenerator : ChunkGenerator {
		private const int BIOME_BLEND_RANGE = 10;
		private readonly BiomeMap biomeMap;
		private readonly Heightmap heightmap;
		private readonly Cavemap cavemap;

		public NoiseLevelChunkGenerator(BiomeMap biomeMap, Heightmap heightmap, Cavemap cavemap) {
			this.biomeMap = biomeMap;
			this.heightmap = heightmap;
			this.cavemap = cavemap;
		}

		public override void Generate(Level level, Chunk chunk, bool placeBlocks) {
			ChunkGenData genData = new() {
				genContexts = new BlockGenContext[Level.CHUNK_LENGTH][],
				surfacePoints = new int[Level.CHUNK_LENGTH],
				biomeWeights = new IEnumerable<BiomeWeight>[Level.CHUNK_LENGTH],
				caveDensities = new float[Level.CHUNK_LENGTH][],
				caveMask = new BitArray[Level.CHUNK_LENGTH]
			};
			ChunkPos chunkPos = chunk.GetPos();

			for (int cx = 0; cx < Level.CHUNK_LENGTH; cx++) {
				genData.caveDensities[cx] = new float[Level.WORLD_HEIGHT];
				genData.caveMask[cx] = new BitArray(Level.WORLD_HEIGHT);
				genData.genContexts[cx] = new BlockGenContext[Level.WORLD_HEIGHT];
				int x = chunkPos.ChunkXToWorldX(cx);

				IEnumerable<BiomeWeight> weights = this.biomeMap.ResolveWeights(x);
				this.biomeMap.ResolvePrimaryBiomes(weights, out var primary, out var secondary);
				genData.biomeWeights[cx] = weights;

				ChunkBiomePartition partition = this.ProcessBiomePartition(x, primary.biome, genData.biomePartition);
				genData.biomePartition = partition;

				int height = Mathf.FloorToInt(this.heightmap.SampleHeight(x, primary, secondary));
				float surfaceY = this.heightmap.ToYCoord(height);

				BlockResolver blockResolver = new(primary.biome, secondary?.biome);

				for (int y = 0; y < Level.WORLD_HEIGHT; y++) {
					BlockPos blockPos = new(x, WorldChunk.IndexToWorldY(y));
					float caveDensity = this.cavemap.SampleDensity(x, blockPos.y, surfaceY, primary, secondary);
					bool isCave = this.cavemap.IsCave(caveDensity);

					BlockGenContext ctx = new() {
						pos = blockPos,
						surfaceY = this.heightmap.ToYCoord(height),
						caveDensity = caveDensity,
						isCave = isCave,
					};

					genData.genContexts[cx][y] = ctx;
					genData.caveDensities[cx][y] = caveDensity;
					genData.caveMask[cx][y] = isCave;
					genData.surfacePoints[cx] = ctx.surfaceY;

					if (placeBlocks) {
						BlockState blockState = blockResolver.ResolveBlock(ctx);
						chunk.SetBlockState(blockPos, blockState);
					}
				}
			}
			if (chunk is WorldChunk worldChunk) {
				worldChunk.surfacePoints = genData.surfacePoints;
			}
			if (placeBlocks) {
				this.BlendBiomeBorder(genData.biomePartition);
				this.PostProcess(genData, level, chunk);
			}
		}

		private ChunkBiomePartition ProcessBiomePartition(int x, IBiome primary, ChunkBiomePartition partition) {
			partition.primary ??= primary;
			if (partition.primary != primary && partition.secondary == null) {
				partition.secondary = primary;
				partition.splitX = x;
			}
			return partition;
		}

		[Obsolete("Biome blending is no longer available due to chunk gen dependency graph rework")]
		private void BlendBiomeBorder(ChunkBiomePartition partition) {
			//if (!partition.hasBorder) {
			//	return;
			//}

			//int leftX = partition.splitX - (BIOME_BLEND_RANGE / 2);
			//int rightX = partition.splitX + (BIOME_BLEND_RANGE / 2) - 1;
			//BlockResolver blockResolver = new(partition.primary, partition.secondary);

			//for (int x = leftX; x <= rightX; x++) {
			//	int cx = ToChunkX(x);
			//	int chunkX = ChunkXAt(x);
			//	int localCx = cx;

			//	OnChunkGenerated onChunkGenerated = genData => {
			//		for (int y = 0; y < WORLD_HEIGHT; y++) {
			//			BlockGenContext ctx = genData.genContexts[localCx][y];
			//			var blockState = blockResolver.BlendBiomeBorder(ctx, leftX, rightX);
			//			genData.chunk.SetBlock(localCx, y, blockState);
			//		}
			//	};

			//	if (this.IsChunkGenerated(chunkX)) {
			//		onChunkGenerated(this.chunkGenData[chunkX]);
			//	} else {
			//		this.PostOnChunkGenerated(chunkX, onChunkGenerated);
			//	}
			//}
		}

		private void PostProcess(ChunkGenData genData, Level level, Chunk chunk) {
			IBiome primary = genData.biomePartition.primary;
			IBiome? secondary = genData.biomePartition.secondary;
			ChunkPos chunkPos = chunk.GetPos();

			int splitX = genData.biomePartition.splitX;
			int chunkStartX = chunkPos.ChunkXToWorldX(0);
			int chunkEndX = chunkPos.ChunkXToWorldX(Level.CHUNK_LENGTH - 1);

			int partitionStartX = chunkStartX;
			int partitionLimitX = secondary == null ? chunkEndX : splitX;
			primary.PostProcess(genData, chunk, level, partitionStartX, partitionLimitX);

			if (secondary != null) {
				partitionStartX = splitX + 1;
				partitionLimitX = chunkEndX;

				secondary?.PostProcess(genData, chunk, level, partitionStartX, partitionLimitX);
			}
		}

	}
}
