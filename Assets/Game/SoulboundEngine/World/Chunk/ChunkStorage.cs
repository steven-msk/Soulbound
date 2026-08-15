using SoulboundEngine.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable

namespace SoulboundEngine.World.Chunk {
	using File = Core.Serialization.File;
	using Level = Level.Level;

	public class ChunkStorage : IDisposable {
		private const string FILE_EXTENSION = ".txt";
		private readonly HashSet<int> savedChunks = new();
		private readonly File folder;

		public ChunkStorage(File folder) {
			if (!folder.IsDirectory) throw new NotSupportedException("File is not folder: " + folder.FullPath);
			folder.Mkdir();
			this.folder = folder;

			foreach (var file in folder.ListFiles()) {
				if (!IsFileValid(file)) {
					Logger.LogError("Chunk file '{}' is not valid, skipping", file.Name);
					continue;
				}
				if (!TryGetChunkX(file, out int chunkX)) {
					Logger.LogError("Failed to get chunkX: {}, skipping this chunk", file.Name);
					continue;
				}
				if (!this.savedChunks.Add(chunkX)) {
					Logger.LogError("Chunk is already present: {}", chunkX);
				}
			}
		}

		private static bool IsFileValid(File file) {
			return TryGetChunkX(file, out int _) && file.Extension == FILE_EXTENSION;
		}

		private static bool TryGetChunkX(File file, out int chunkX) {
			return int.TryParse(file.NameWithoutExtension, out chunkX);
		}

		public bool Has(int chunkX) {
			return this.savedChunks.Contains(chunkX);
		}

		public Chunk? Read(Level level, int chunkX) {
			if (!this.Has(chunkX)) return null;

			File chunkFile = ToChunkFile(chunkX, this.folder);
			ChunkPos chunkPos = new(chunkX);

			using StreamReader chunkReader = chunkFile.OpenText();
			string json = chunkReader.ReadToEnd();

			SerializableChunkData data = SerializableChunkData.Parse(json, level);
			return data.Read(level, chunkPos);
		}

		public void Save(Level level, Chunk chunk) {
			File chunkFile = ToChunkFile(chunk, this.folder);
			chunkFile.CreateNewFile();
			using StreamWriter writer = chunkFile.CreateText();

			SerializableChunkData data = SerializableChunkData.Of(level, chunk);
			writer.Write(data.Write());

			int chunkX = chunk.GetPos().x;
			this.savedChunks.Add(chunkX);
		}

		public void Dispose() {
		}

		public static File ToChunkFile(Chunk chunk, File parent) {
			return ToChunkFile(chunk.GetPos().x, parent);
		}

		public static File ToChunkFile(int chunkX, File parent) {
			return parent.Combine(chunkX.ToString()).WithExtension(FILE_EXTENSION);
		}
	}
}
