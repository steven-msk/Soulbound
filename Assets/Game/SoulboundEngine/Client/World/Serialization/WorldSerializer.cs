using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.Level;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoulboundEngine.Client.World.Serialization {
	using Chunk = Chunk.Chunk;
	using File = Core.Serialization.File;
	using Level = Level.Level;

	public class WorldSerializer : IWorldSaveValidator {
		public const string SEED_FILE_NAME = "seed.txt";
		private const string CHUNK_FILE_EXTENSION = ".txt";
		private const string CHUNK_INDEX_FILE = "chunkIndex.txt";
		private const char CHUNK_INDEX_SEPARATOR = ' ';

		// the current implementation assumes there is only one Level at all times
		// since multiple dimensions are planned, this will need a revisit

		[Obsolete]
		public void Serialize(LevelManager levelManager, File saveFolder) {
			//Level level = levelManager.GetLevel();
			//IEnumerable<Chunk> chunks = level.GetGeneratedChunks();
			//WriteChunks(level, chunks, saveFolder);
		}

		private static void WriteChunks(Level level, IEnumerable<Chunk> chunks, File saveFolder) {
			File indexFile = ToChunkIndexFile(saveFolder);
			indexFile.CreateNewFile();
			using StreamWriter indexWriter = indexFile.CreateText();

			bool first = true;
			foreach (var chunk in chunks) {
				if (!first) indexWriter.Write(CHUNK_INDEX_SEPARATOR);
				indexWriter.Write(chunk.GetPos().x);
				first = false;
				WriteChunk(level, chunk, saveFolder);
			}
		}

		private static void WriteChunk(Level level, Chunk chunk, File saveFolder) {
			File chunkFile = ToChunkFile(chunk, saveFolder);
			chunkFile.CreateNewFile();
			using StreamWriter writer = chunkFile.CreateText();

			SerializableChunkData data = SerializableChunkData.Of(level, chunk);
			writer.Write(data.Write());
		}

		[Obsolete]
		public void Deserialize(LevelManager levelManager, File saveFolder) {
			//Level level = levelManager.GetLevel();
			//ReadChunks(level, saveFolder);
		}

		private static void ReadChunks(Level level, File saveFolder) {
			File indexFile = ToChunkIndexFile(saveFolder);
			if (!indexFile.Exists) {
				throw new InvalidOperationException("No chunk index file found in path " + saveFolder.FullPath);
			}
			List<Chunk> deserializedChunks = new();

			using StreamReader reader = indexFile.OpenText();
			string line = reader.ReadLine();
			if (string.IsNullOrEmpty(line)) return;

			string[] split = line.Split(CHUNK_INDEX_SEPARATOR);
			for (int i = 0; i < split.Length; i++) {
				int chunkX = int.Parse(split[i]);
				File chunkFile = ToChunkFile(chunkX, saveFolder);

				using StreamReader chunkReader = chunkFile.OpenText();
				Chunk chunk = ReadChunk(level, chunkReader, new ChunkPos(chunkX));
				deserializedChunks.Add(chunk);
			}
			level.ReplaceGenerated(deserializedChunks);
		}

		private static Chunk ReadChunk(Level level, StreamReader reader, ChunkPos chunkPos) {
			string json = reader.ReadToEnd();
			SerializableChunkData data = SerializableChunkData.Parse(json, level);
			return data.Read(level, chunkPos);
		}

		private static File ToChunkFile(Chunk chunk, File saveFolder) {
			return ToChunkFile(chunk.GetPos().x, saveFolder);
		}

		private static File ToChunkFile(int chunkX, File saveFolder) {
			return saveFolder.Combine(chunkX.ToString()).WithExtension(CHUNK_FILE_EXTENSION);
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
