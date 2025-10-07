using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Noise;
using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.Chunk {
	[JsonConverter(typeof(WorldChunkJsonConverter))]
	public class WorldChunk {
		public static readonly int minY = -Level.WORLD_HEIGHT / 2;
		public static readonly int maxY = Level.WORLD_HEIGHT / 2;
		public const float HEIGHT_SPREAD = 0.01f;
		public const float SURFACE_HEIGHT_RANGE = 50f;
		public const float UNDERGROUND_HEIGHT_RANGE = 20f;

		private ChunkHeightmapData heightmapData;
		public ChunkHeightmapData HeightmapData => heightmapData;

		private int x;
		public int xpos => x;

		private BlockState[][] blockStates = new BlockState[Level.CHUNK_LENGTH][];

		public WorldChunk(int x) { 
			this.x = x;
			for (int cx = 0; cx < Level.CHUNK_LENGTH; cx++) {
				blockStates[cx] = new BlockState[Level.WORLD_HEIGHT];
			}
		}

		// PLANNED REFACTOR: chunk generation logic - required when introducing biomes
		public ChunkHeightmapData GenerateHeightmap(INoiseGenerator1D heightGenerator) {
			int startX = x * Level.CHUNK_LENGTH;
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
					} else if (y == groundHeight) {
						blockState = Blocks.grass.defaultState;
					} else if (y < groundHeight && y >= undergroundHeight) {
						blockState = Blocks.dirt.defaultState;
					} else {
						blockState = Blocks.stone.defaultState;
					}
					blockStates[x][yIndex] = blockState;
				}
			}

			ChunkHeightmapData generationData = new ChunkHeightmapData(surfaceLevels, highestStone);
			this.heightmapData = generationData;
			return generationData;
		}

		int WorldYToIndex(int worldY) => worldY - minY;

		int IndexToWorldY(int yIndex) => yIndex + minY;

		public void Render(Tilemap tilemap, ChunkOutlineRenderer outlineRenderer) {
			int xStart = x * Level.CHUNK_LENGTH;
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = minY; y < maxY; y++) {
					int yIndex = WorldYToIndex(y);
					BlockState blockState = blockStates[x][yIndex];
					InvocationHelper.IfElse(blockState != null,
						() => tilemap.SetTile(new Vector3Int(xStart + x, y), blockState.block.tileReference),
						() => UnityEngine.Debug.LogError($"Attempted to render ungenerated terrain! {new ChunkBlockPos(x, y, this.x).ToString()}"));
				}
			}
			this.RefreshTiles(tilemap);
			outlineRenderer.ShowOutline(this);
		}

		public void RefreshTiles(Tilemap tilemap) {
			int xStart = x * Level.CHUNK_LENGTH;
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = minY; y < maxY; y++) {
					int yIndex = WorldYToIndex(y);
					tilemap.RefreshTile(new Vector3Int(xStart + x, y, 0));
				}
			}
		}

		public void Unload(Tilemap tilemap, ChunkOutlineRenderer outlineRenderer) {
			int xStart = x * Level.CHUNK_LENGTH;
			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				for (int y = minY; y < maxY; y++) {
					tilemap.SetTile(new Vector3Int(xStart + x, y, 0), null);
				}
			}
			outlineRenderer.HideOutline(this);
		}

		public void SetBlock(ChunkBlockPos chunkPos, BlockState blockState) {
			blockStates[chunkPos.x][WorldYToIndex(chunkPos.y)] = blockState;
		}

		public BlockState BlockStateAt(ChunkBlockPos chunkPos) { 
			return blockStates[chunkPos.x][WorldYToIndex(chunkPos.y)];
		}

		public sealed class WorldChunkJsonConverter : JsonConverter<WorldChunk> {
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
				BlockState[][] blockStates = new BlockState[rows.Count][];
				for (int x = 0; x < rows.Count; x++) {
					JArray row = (JArray)rows[x];
					blockStates[x] = new BlockState[row.Count];
					for (int y = 0; y < row.Count; y++) {
						int blockID = row[y]!.Value<int>();
						Block block = Blocks.ByID(blockID);
						blockStates[x][y] = block.defaultState;
					}
				}

				return new WorldChunk(xpos) {
					heightmapData = heightmap,
					blockStates = blockStates
				};
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
				foreach (var row in value.blockStates) {
					writer.WriteStartArray();
					foreach (var blockState in row) {
						serializer.Serialize(writer, blockState);
					}
					writer.WriteEndArray();
				}
				writer.WriteEndArray();

				writer.WriteEndObject();
			}
		}
	}
}
