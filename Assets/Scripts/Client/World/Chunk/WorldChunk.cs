using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Generation;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Noise;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.Tilemaps;
using Logger = SoulboundBackend.Common.Logging.Logger;
using TerrainData = SoulboundBackend.Client.World.Generation.TerrainData;

namespace SoulboundBackend.Client.World.Chunk {
	[JsonConverter(typeof(WorldChunk.Serializer))]
	public class WorldChunk : ITickable {
		private static readonly Logger logger = Logger.CreateInstance();
		public const int minY = -Level.WORLD_HEIGHT / 2;
		public const int maxY = Level.WORLD_HEIGHT / 2;
		public const float HEIGHT_SPREAD = 0.01f;
		public const float SURFACE_HEIGHT_RANGE = 50f;
		public const float UNDERGROUND_HEIGHT_RANGE = 20f;

		private readonly int[][] stateHashes = new int[Level.CHUNK_LENGTH][];
		private readonly TileEntity[][] tileEntities = new TileEntity[Level.CHUNK_LENGTH][];
		private readonly TileEntityTickManager tickManager = new();
		private ChunkHeightmapData heightmapData;
		public ChunkHeightmapData HeightmapData => heightmapData;
		private int cx;
		public int xpos => cx;

		public WorldChunk(int cx) { 
			this.cx = cx;

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				tileEntities[x] = new TileEntity[Level.WORLD_HEIGHT];
				stateHashes[x] = new int[Level.WORLD_HEIGHT];
			}
		}

		public void Tick() => tickManager.Tick();

		// PLANNED REFACTOR: chunk generation logic - required when introducing biomes
		[Obsolete]
		public ChunkHeightmapData GenerateHeightmap(INoiseGenerator1D heightGenerator) {

			int startX = cx * Level.CHUNK_LENGTH;
			Dictionary<int, int> surfaceLevels = new();
			int highestStone = 0;

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				int worldX = startX + x;
				float heightNoise = heightGenerator.GenerateNoise1D(worldX);
				int groundHeight = Mathf.FloorToInt(heightNoise * SURFACE_HEIGHT_RANGE);
				int undergroundHeight = Mathf.FloorToInt(heightNoise * UNDERGROUND_HEIGHT_RANGE);
				surfaceLevels.Add(worldX, groundHeight + 1);

				highestStone = Mathf.Max(highestStone, undergroundHeight);
				for (int y = minY; y < maxY; y++) {
					int yIndex = WorldYToIndex(y);
					BlockState blockState = default(BlockState);
					if (y > groundHeight) {
						blockState = Blocks.air.defaultState;
					} else if (y == groundHeight || y == groundHeight - 1) {
						blockState = Blocks.grass.defaultState;
					} else if (y < groundHeight - 1 && y >= undergroundHeight) {
						blockState = Blocks.dirt.defaultState;
					} else {
						blockState = Blocks.stone.defaultState;
					}
					stateHashes[x][yIndex] = blockState.stateHash;
				}
			}



			ChunkHeightmapData generationData = new ChunkHeightmapData(surfaceLevels, highestStone);
			this.heightmapData = generationData;
			return generationData;
		}

		public void Generate(BiomeMap biomeMap, Heightmap heightmap, Cavemap cavemap, out TerrainData terrainData) {
			terrainData = new TerrainData {
				chunk = this,
				surfacePoints = new Dictionary<int, int>(),
				biomeWeights = new Dictionary<int, IEnumerable<BiomeWeight>>(),
				caveDensities = new float[Level.CHUNK_LENGTH][],
				caveMask = new BitArray[Level.CHUNK_LENGTH]
			};

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				terrainData.caveDensities[x] = new float[Level.WORLD_HEIGHT];
				terrainData.caveMask[x] = new BitArray(Level.WORLD_HEIGHT);

				int blockX = ChunkXToWorldX(x);
				var weights = biomeMap.ResolveWeights(blockX);
				biomeMap.ResolvePrimaryBiomes(weights, out var primary, out var secondary);
				float height = heightmap.SampleHeight(blockX, primary, secondary);
				float surfaceY = heightmap.ToYCoord(height);

				IBiome biome = primary.biome;
				terrainData.biomeWeights[blockX] = weights;

				for (int y = 0; y < Level.WORLD_HEIGHT; y++) {
					BlockPos blockPos = new(blockX, IndexToWorldY(y));
					float caveDensity = cavemap.SampleDensity(blockX, blockPos.y, surfaceY, primary, secondary);
					bool isCave = cavemap.IsCave(caveDensity);

					BlockContext ctx = new BlockContext {
						pos = new BlockPos(blockX, IndexToWorldY(y)),
						surfaceY = heightmap.ToYCoord(height),
						caveDensity = caveDensity,
						isCave = isCave,
					};


					terrainData.caveDensities[x][y] = caveDensity;
					terrainData.caveMask[x][y] = isCave;
					terrainData.surfacePoints[blockX] = Mathf.FloorToInt(ctx.surfaceY);

					stateHashes[x][y] = !isCave
						? biome.ResolveBlock(ctx).stateHash
						: biome.ResolveCaveBlock(blockPos, caveDensity).stateHash;
				}
			}
		}

		public void PostProcessTerrain(TerrainData data, Level level) {
			Dictionary<IBiome, List<BiomeInterval>> biomeIntervals = new();

			for (int cx = 0; cx < Level.CHUNK_LENGTH; cx++) {
				int blockX = ChunkXToWorldX(cx);
				var weights = data.biomeWeights[blockX];
				var primary = weights.First(w => w.value == 1f);
				
				if (!biomeIntervals.TryGetValue(primary.biome, out var list)) {
					list = new List<BiomeInterval>();
					biomeIntervals[primary.biome] = list;
					list.Add(new BiomeInterval { 
						startXInclusive = blockX, 
						endXExclusive = blockX + 1
					});
				} else {
					var last = list.Last();
					if  (last.endXExclusive == blockX) {
						last.endXExclusive = blockX + 1;
					} else {
						list.Add(new BiomeInterval {
							startXInclusive = blockX,
							endXExclusive = blockX + 1
						});
					}
				}
			}

			foreach (var (biome, intervals) in biomeIntervals) {
				biome.PostProcessTerrain(data, this, level, intervals);
			}
		}

		public static int WorldYToIndex(int worldY) => worldY - minY;

		public static int IndexToWorldY(int yIndex) => yIndex + minY;

		public int WorldXToChunkX(int x) => x - xpos * Level.CHUNK_LENGTH;

		public int ChunkXToWorldX(int cx) => cx + xpos * Level.CHUNK_LENGTH;

		public void Render(Tilemap tilemap, ChunkOutlineRenderer outlineRenderer) {
			int xStart = cx * Level.CHUNK_LENGTH;

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = minY; y < maxY; y++) {
					int yIndex = WorldYToIndex(y);
					int stateHash = stateHashes[x][yIndex];

					if (stateHash != 0) {
						BlockState state = BlockStateRegistry.Get(stateHash);
						BlockPos pos = new(xStart + x, y);

						state.block.Render(state, tileEntities[x][yIndex], pos, tilemap);
					} else {
						UnityEngine.Debug.LogError($"Attempted to render ungenerated terrain! {new ChunkBlockPos(x, y, this.cx).ToString()}");
					}
				}
			}

			this.RefreshTiles(tilemap);
			outlineRenderer.ShowOutline(this);
		}

		public void RefreshTiles(Tilemap tilemap) {
			int xStart = cx * Level.CHUNK_LENGTH;

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = minY; y < maxY; y++) {
					int yIndex = WorldYToIndex(y);
					tilemap.RefreshTile(new Vector3Int(xStart + x, y, 0));
				}
			}
		}

		public void Unload(Tilemap tilemap, ChunkOutlineRenderer outlineRenderer) {
			int xStart = cx * Level.CHUNK_LENGTH;
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = minY; y < maxY; y++) {
					tilemap.SetTile(new Vector3Int(xStart + x, y, 0), null);
				}
			}
			outlineRenderer.HideOutline(this);
		}

		public void SetBlock(ChunkBlockPos chunkPos, BlockState blockState) {
			var xIndex = chunkPos.x;
			var yIndex = WorldYToIndex(chunkPos.y);
			Block block = blockState.block;

			stateHashes[xIndex][yIndex] = blockState.stateHash;
			if (block.hasTileEntity) {
				TileEntity tileEntity = block.GetTileEntity(this, chunkPos.ToBlockPos());
				
				if (tileEntities[xIndex][yIndex] != null) {
					tickManager.RemoveTileEntity(tileEntity);
				}
				
				tileEntities[xIndex][yIndex] = tileEntity;
				tickManager.AddTileEntity(tileEntity);
			}
		}

		public BlockState BlockStateAt(ChunkBlockPos chunkPos) {
			int stateHash = stateHashes[chunkPos.x][WorldYToIndex(chunkPos.y)];
			if (BlockStateRegistry.TryGet(stateHash, out var state)) {
				return state;
			}
			return Blocks.air.defaultState;
		}

		private void ParseDeserialized(int[][] stateHashes) {
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = minY; y < maxY; y++) {
					int yIndex = WorldYToIndex(y);

					this.stateHashes[x][yIndex] = stateHashes[x][yIndex];
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
				ChunkHeightmapData heightmap = obj["heightmapData"].ToObject<ChunkHeightmapData>(serializer);
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

				WorldChunk chunk = new(xpos) {
					heightmapData = heightmap,
				};
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

				writer.WritePropertyName("heightmapData");
				serializer.Serialize(writer, value.HeightmapData);

				writer.WritePropertyName("blockStates");
				writer.WriteStartArray();
				foreach (var row in value.stateHashes) {
					writer.WriteStartArray();
					foreach (var stateHash in row) {
						writer.WriteValue(stateHash);
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
