using SoulboundEngine.Client.Debug.Logging;
using System;

namespace SoulboundEngine.Client.World.Serialization {
	using File = Core.Serialization.File;

	public class WorldSaveValidator : IWorldSaveValidator {
		private readonly string seedFileName;
		private readonly string chunksFolderName;

		public WorldSaveValidator(string seedFileName, string chunksFolderName) {
			this.seedFileName = seedFileName;
			this.chunksFolderName = chunksFolderName;
		}

		public bool IsValid(File saveFolder) {
			return saveFolder.HasChild(this.seedFileName);
		}

		public void ValidateNewSave(File saveFolder, int seed) {
			File seedFile = saveFolder.Combine(this.seedFileName);
			if (!seedFile.CreateNewFile()) {
				throw new InvalidOperationException("Failed to create seed file: " + seedFile.FullPath);
			}
			seedFile.WriteAllText(seed.ToString());

			File chunksFolder = saveFolder.Combine(this.chunksFolderName);
			chunksFolder.Mkdir();
		}

		public bool Validate(File saveFolder, out int seed, out string worldName, out File chunksFolder) {
			seed = 0;
			worldName = saveFolder.Name;
			chunksFolder = default;

			File seedFile = saveFolder.Combine(this.seedFileName);
			if (!seedFile.Exists) {
				Logger.LogError("Save validation failed: seed file does not exist: " + seedFile.FullPath);
				return false;
			}
			try {
				string seedText = seedFile.ReadAllText();
				seed = int.Parse(seedText);
			} catch (Exception e) {
				Logger.LogFatal(e, "Save validation failed: could not parse seed");
				return false;
			}

			chunksFolder = saveFolder.Combine(this.chunksFolderName);
			chunksFolder.Mkdir();
			return true;
		}
	}
}
