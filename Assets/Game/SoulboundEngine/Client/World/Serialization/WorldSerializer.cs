using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SoulboundEngine.Client.World.Serialization {
	using Block = Block.Block;
	using File = Core.Serialization.File;
	using Level = Level.Level;

	public class WorldSerializer : IWorldSaveValidator {
		public const string SEED_FILE_NAME = "seed.txt";
		private const string CHUNK_FILE_EXTENSION = ".txt";
		private const char STATE_ID_SEPARATOR = ' ';
		private const string TILE_ENTITIES_FILE = "tileEntities.json";
		private const string CHUNK_INDEX_FILE = "chunkIndex.txt";
		private const char CHUNK_INDEX_SEPARATOR = ' ';

		// the current implementation assumes there is only one Level at all times
		// since multiple dimensions are planned, this will need a revisit

		public void Serialize(LevelManager levelManager, File saveFolder) {
			Level level = levelManager.GetLevel();
			IEnumerable<WorldChunk> chunks = level.GetGeneratedChunks();
			WriteChunks(chunks, saveFolder);

			List<TileEntity> tileEntities = new();
			foreach (var chunk in chunks) {
				tileEntities.AddRange(chunk.GetTileEntities());
			}
			WriteTileEntities(tileEntities, saveFolder);
		}

		private static void WriteChunks(IEnumerable<WorldChunk> chunks, File saveFolder) {
			File indexFile = ToChunkIndexFile(saveFolder);
			indexFile.CreateNewFile();
			using StreamWriter indexWriter = indexFile.CreateText();

			bool first = true;
			foreach (var chunk in chunks) {
				if (!first) indexWriter.Write(CHUNK_INDEX_SEPARATOR);
				indexWriter.Write(chunk.chunkX);
				first = false;
				WriteChunk(chunk, saveFolder);
			}
		}

		private static void WriteChunk(WorldChunk chunk, File saveFolder) {
			File chunkFile = ToChunkFile(chunk, saveFolder);
			chunkFile.CreateNewFile();

			using StreamWriter writer = chunkFile.CreateText();
			WriteBlocks(chunk, writer);
		}

		private static void WriteBlocks(WorldChunk chunk, TextWriter writer) {
			int[][] stateIDs = chunk.GetBlocks();

			int totalBlocks = stateIDs.Sum(row => row.Length);
			StringBuilder stringBuilder = new(totalBlocks * 10);  // 10 chars per int + separator, rough estimate
			bool first = true;

			foreach (int[] row in stateIDs) {
				foreach (int stateId in row) {
					if (!first) stringBuilder.Append(STATE_ID_SEPARATOR);
					stringBuilder.Append(stateId);
					first = false;
				}
			}

			writer.WriteLine(stringBuilder.ToString());
		}

		private static void WriteTileEntities(List<TileEntity> tileEntities, File saveFolder) {
			File tileEntitiesFile = ToTileEntitiesFile(saveFolder);
			tileEntitiesFile.CreateNewFile();
			using StreamWriter writer = tileEntitiesFile.CreateText();

			JArray objArray = new();
			foreach (var tileEntity in tileEntities) {
				JObject obj = WriteTileEntity(tileEntity);
				objArray.Add(obj);
			}
			string json = JsonConvert.SerializeObject(objArray, new JsonSerializerSettings() { Formatting = Formatting.Indented });
			writer.Write(json);
		}

		private static JObject WriteTileEntity(TileEntity tileEntity) {
			JObject json = new() {
				["type"] = TileEntityType.GetId(tileEntity.GetTileEntityType())!.ToString(),
				["pos"] = tileEntity.blockPos.ToString(),
			};
			tileEntity.Write(json);
			return json;
		}

		public void Deserialize(LevelManager levelManager, File saveFolder) {
			Dictionary<int, int[][]> stateIDsByChunk = new();
			List<TileEntity> tileEntities = new();

			ReadBlocks(stateIDsByChunk, saveFolder);
			ReadTileEntities(tileEntities, saveFolder, stateIDsByChunk);

			Level level = levelManager.GetLevel();
			level.ApplyDeserializedBlocks(stateIDsByChunk);
			level.ApplyDeserializedTileEntities(tileEntities);
			level.SyncBlocksWithTileEntities();
		}

		private static void ReadBlocks(Dictionary<int, int[][]> stateIDsByChunk, File saveFolder) {
			File indexFile = ToChunkIndexFile(saveFolder);
			if (!indexFile.Exists) {
				throw new InvalidOperationException("No chunk index file found in path " + saveFolder.FullPath);
			}

			using StreamReader reader = indexFile.OpenText();
			string line = reader.ReadLine();

			string[] split = line.Split(CHUNK_INDEX_SEPARATOR);
			for (int i = 0; i < split.Length; i++) {
				int chunkX = int.Parse(split[i]);

				File chunkFile = ToChunkFile(chunkX, saveFolder);
				using StreamReader chunkReader = chunkFile.OpenText();
				ReadBlocksInChunk(stateIDsByChunk, chunkX, chunkReader);
			}
		}

		private static void ReadBlocksInChunk(Dictionary<int, int[][]> stateIDsByChunk, int chunkX, StreamReader reader) {
			string line = reader.ReadLine();
			string[] ids = line.Split(STATE_ID_SEPARATOR);

			int[][] stateIDs = default;
			WorldChunk.CreateBlockArray(ref stateIDs);

			for (int i = 0; i < stateIDs.Length; i++) {
				for (int j = 0; j < stateIDs[i].Length; j++) {
					stateIDs[i][j] = int.Parse(ids[i * stateIDs[i].Length + j]);
				}
			}

			stateIDsByChunk.Add(chunkX, stateIDs);
		}

		private static void ReadTileEntities(List<TileEntity> tileEntities, File saveFolder, Dictionary<int, int[][]> stateIDsByChunk) {
			File tileEntitiesFile = ToTileEntitiesFile(saveFolder);
			if (!tileEntitiesFile.Exists) return;   // not required for a valid save

			try {
				string json = tileEntitiesFile.ReadAllText();
				JArray objArray = JArray.Parse(json);

				foreach (var token in objArray) {
					ReadTileEntity(tileEntities, token, stateIDsByChunk);
				}
			} catch (Exception e) {
				Logger.LogFatal(e);
			}
		}

		private static void ReadTileEntity(List<TileEntity> tileEntities, JToken json, Dictionary<int, int[][]> stateIDsByChunk) {
			try {
				Identifier typeId = Identifier.Of((string)json["type"]);
				TileEntityType type = Registries.TILE_ENTITIES.Get(typeId);

				BlockPos blockPos = BlockPos.Parse((string)json["pos"]);
				BlockState state = ResolveDeserializedState(stateIDsByChunk, blockPos);
				if (!type.Supports(state)) {
					throw new InvalidOperationException($"Tile entity {typeId} " +
						$"does not support deserialized block state {state}");
				}

				TileEntity tileEntity = type.Instantiate(blockPos, state);
				tileEntity.Read(json);
				tileEntities.Add(tileEntity);
			} catch (Exception e) {
				Logger.LogFatal(e);
			}
		}

		private static BlockState ResolveDeserializedState(Dictionary<int, int[][]> stateIDsByChunk, BlockPos blockPos) {
			ChunkBlockPos chunkPos = ChunkBlockPos.FromBlockPos(blockPos);
			if (!stateIDsByChunk.TryGetValue(chunkPos.chunkX, out int[][] stateIDs)) {
				return Blocks.AIR.DefaultState;
			}

			int stateID = stateIDs[chunkPos.x][WorldChunk.WorldYToIndex(chunkPos.y)];
			return Block.GetState(stateID);
		}

		private static File ToChunkFile(WorldChunk chunk, File saveFolder) {
			return ToChunkFile(chunk.chunkX, saveFolder);
		}

		private static File ToChunkFile(int chunkX, File saveFolder) {
			return saveFolder.Combine(chunkX.ToString()).WithExtension(CHUNK_FILE_EXTENSION);
		}

		private static File ToTileEntitiesFile(File saveFolder) {
			return saveFolder.Combine(TILE_ENTITIES_FILE);
		}

		private static File ToChunkIndexFile(File saveFolder) {
			return saveFolder.Combine(CHUNK_INDEX_FILE);
		}

		bool IWorldSaveValidator.IsValid(File saveFolder) {
			return saveFolder.HasChild(SEED_FILE_NAME) && saveFolder.HasChild(CHUNK_INDEX_FILE);
		}

		void IWorldSaveValidator.ValidateNewSave(File saveFolder, int seed) {
			File seedFile = saveFolder.Combine(SEED_FILE_NAME);
			if (!seedFile.CreateNewFile()) {
				throw new InvalidOperationException("Failed to create seed file: " + seedFile.FullPath);
			}
			seedFile.WriteAllText(seed.ToString());

			File indexFile = saveFolder.Combine(CHUNK_INDEX_FILE);
			if (!indexFile.CreateNewFile()) {
				throw new InvalidOperationException("Failed to create index file: " + indexFile.FullPath);
			}
		}
	}
}
