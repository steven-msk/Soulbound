using Assets.Scripts.Client.World.Biome;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Generation;
using SoulboundBackend.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


#nullable enable

namespace SoulboundBackend.Client.World.Chunk {
	[JsonConverter(typeof(WorldChunk.Serializer))]
	public class WorldChunk : ITickable {
		public const int minY = -Level.WORLD_HEIGHT / 2;
		public const int maxY = Level.WORLD_HEIGHT / 2;
		public const float HEIGHT_SPREAD = 0.01f;
		public const float SURFACE_HEIGHT_RANGE = 50f;
		public const float UNDERGROUND_HEIGHT_RANGE = 20f;

		private readonly int[][] blockStateIDs = new int[Level.CHUNK_LENGTH][];
		private readonly Dictionary<BlockPos, TileEntity> tileEntities = new();
		private readonly TileEntityTickManager tickManager = new();
		private readonly Level level;
		private readonly int cx;
		public int xpos => cx;

		public WorldChunk(Level level, int cx) { 
			this.level = level;
			this.cx = cx;

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				blockStateIDs[x] = new int[Level.WORLD_HEIGHT];
			}
		}

		public void Tick() => tickManager.Tick();

		[Obsolete]
		public void Generate(BiomeMap biomeMap, Heightmap heightmap, Cavemap cavemap, out ChunkGenData genData) {
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
				int x = ChunkXToWorldX(cx);

				var weights = biomeMap.ResolveWeights(x);
				biomeMap.ResolvePrimaryBiomes(weights, out var primary, out var secondary);
				genData.biomeWeights[cx] = weights;

				var partition = ProcessBiomePartition(x, primary.biome, genData.biomePartition);
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

					BlockState blockState = blockResolver.ResolveBlock(ctx);
					SetBlock(cx, y, blockState);
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
			int chunkStartX = ChunkXToWorldX(0);
			int chunkEndX = ChunkXToWorldX(Level.CHUNK_LENGTH - 1);

			int partitionStartX = chunkStartX;
			int partitionLimitX = secondary == null ? chunkEndX : splitX;
			primary.PostProcess(genData, this, level, partitionStartX, partitionLimitX);

			if (secondary != null) {
				partitionStartX = splitX + 1;
				partitionLimitX = chunkEndX;

				secondary?.PostProcess(genData, this, level, partitionStartX, partitionLimitX);
			}
		}

		public static int WorldYToIndex(int worldY) => worldY - minY;

		public static int IndexToWorldY(int yIndex) => yIndex + minY;

		public int WorldXToChunkX(int x) => x - xpos * Level.CHUNK_LENGTH;

		public int ChunkXToWorldX(int cx) => cx + xpos * Level.CHUNK_LENGTH;

		public void OnLoad(ChunkOutlineRenderer outlineRenderer) {
			outlineRenderer.ShowOutline(this);
		}

		public void OnUnload(ChunkOutlineRenderer outlineRenderer) {
			outlineRenderer.HideOutline(this);
		}


		public void SetBlockState(BlockPos blockPos, BlockState? blockState) {
			blockState ??= Blocks.air.defaultState;

			ChunkBlockPos chunkPos = blockPos.ToChunkPos();
			int yIndex = WorldYToIndex(chunkPos.y);
			BlockState oldState = GetBlockState(chunkPos) ?? Blocks.air.defaultState;
			Block oldBlock = oldState.block;
			Block newBlock = blockState.block;

			blockStateIDs[chunkPos.x][yIndex] = blockState.stateID;

			// tile entities only change when blocks differ in type
			// however some blocks may handle tile entity persistence differently
			// when oldBlock and newBlock are the same
			if (newBlock != oldBlock) {
				bool oldHasTileEntity = oldBlock.HasTileEntity(level, blockPos, oldState);
				bool newHasTileEntity = blockState.block.HasTileEntity(level, blockPos, blockState);

				if (oldHasTileEntity && tileEntities.ContainsKey(blockPos)) {
					tickManager.RemoveTileEntity(tileEntities[blockPos]);
					tileEntities.Remove(blockPos);
				}
				if (newHasTileEntity) {
					TileEntity tileEntity = newBlock.GetTileEntity(level, blockPos);
					tileEntities[blockPos] = tileEntity;
					tickManager.AddTileEntity(tileEntity);
				}
			}
		}


		[Obsolete]
		public void SetBlock(ChunkBlockPos chunkPos, BlockState blockState) {
			this.SetBlock(chunkPos.x, WorldYToIndex(chunkPos.y), blockState);
		}
		[Obsolete]
		public void SetBlock(int cx, int yIndex, BlockState blockState) {
			SetBlockState(new BlockPos(ChunkXToWorldX(cx), IndexToWorldY(yIndex)), blockState);
			//Block block = blockState.block;
			//blockStateIDs[cx][yIndex] = blockState.stateID;

			//if (block.hasTileEntity) {
			//	BlockPos blockPos = new(ChunkXToWorldX(cx), IndexToWorldY(yIndex));
			//	TileEntity? tileEntity = block.GetTileEntity(level, blockPos);

			//	if (tileEntities[cx][yIndex] != null) {
			//		tickManager.RemoveTileEntity(tileEntity);
			//	}

			//	tileEntities[cx][yIndex] = tileEntity;
			//	tickManager.AddTileEntity(tileEntity);
			//}
		}

		public BlockState? GetBlockState(ChunkBlockPos chunkPos) {
			if (!Level.IsInBounds(chunkPos.ToBlock())) return null;

			int stateID = blockStateIDs[chunkPos.x][WorldYToIndex(chunkPos.y)];
			return BlockStateRegistry.Get(stateID);
		}

		public TileEntity? TileEntityAt(BlockPos blockPos) {
			return tileEntities.TryGetValue(blockPos, out TileEntity tileEntity)
				? tileEntity
				: null;
		}

		private void ParseDeserialized(int[][] blockStateIDs) {
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = minY; y < maxY; y++) {
					int yIndex = WorldYToIndex(y);

					this.blockStateIDs[x][yIndex] = blockStateIDs[x][yIndex];
				}
			}
		}


		public sealed class Serializer : JsonConverter<WorldChunk> {
			public override WorldChunk ReadJson(
					JsonReader reader,
					Type objectType, 
					WorldChunk existingValue, 
					bool hasExistingValue,
					JsonSerializer serializer
				) {
				if (reader.TokenType == JsonToken.Null) {
					return null;
				}

				JObject obj = JObject.Load(reader);
				int xpos = obj["xpos"].Value<int>();
				JArray rows = (JArray)obj["blockStates"];
				int[][] stateHashes = new int[rows.Count][];

				for (int x = 0; x < rows.Count; x++) {
					JArray row = (JArray)rows[x];
					stateHashes[x] = new int[row.Count];

					for (int y = 0; y < row.Count; y++) {
						int hash = row[y].Value<int>();
						stateHashes[x][y] = hash;
					}
				}

				WorldChunk chunk = new(null, xpos);
				chunk.ParseDeserialized(stateHashes);
				return chunk;
			}

			public override void WriteJson(JsonWriter writer, WorldChunk value, JsonSerializer serializer) {
				if (value == null) {
					writer.WriteNull();
					return;
				}
				writer.WriteStartObject();
				writer.WritePropertyName("xpos");
				serializer.Serialize(writer, value.xpos);

				writer.WritePropertyName("blockStates");
				writer.WriteStartArray();
				foreach (var row in value.blockStateIDs) {
					writer.WriteStartArray();
					foreach (var stateID in row) {
						writer.WriteValue(stateID);
					}
					writer.WriteEndArray();
				}
				writer.WriteEndArray();

				writer.WriteEndObject();
			}

			public static void WriteBinary(BinaryWriter writer, WorldChunk chunk) {
				throw new NotImplementedException();
			}

			public static WorldChunk ReadBinary(BinaryReader reader) {
				throw new NotImplementedException();
			}
		}
	}
}
