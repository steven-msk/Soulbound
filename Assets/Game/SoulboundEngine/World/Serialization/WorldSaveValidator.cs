namespace SoulboundEngine.World.Serialization {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Gen;
	using System;
	using File = SoulboundEngine.Serialization.File;

	public class WorldSaveValidator : IWorldSaveValidator {
		private readonly string propertiesFile;
		private readonly string chunksFolder;

		public WorldSaveValidator(string propertiesFile, string chunksFolder) {
			this.propertiesFile = propertiesFile;
			this.chunksFolder = chunksFolder;
		}

		public bool IsValid(File saveFolder) {
			return saveFolder.HasChild(this.propertiesFile);
		}

		public void ValidateNewSave(File saveFolder, int seed, WorldPreset preset) {
			File propertiesFile = saveFolder.Combine(this.propertiesFile);
			if (!propertiesFile.CreateNewFile()) {
				throw new InvalidOperationException("Failed to create properties file: " + propertiesFile.FullPath);
			}
			LevelPropertyInfo info = new(seed, preset, GlobalSaveVersion.Current);
			propertiesFile.WriteAllText(LevelPropertyInfo.CODEC.Encode(info).ToString(Formatting.Indented));

			File chunksFolder = saveFolder.Combine(this.chunksFolder);
			chunksFolder.Mkdir();
		}

		public WorldSave? Validate(File saveFolder) {
			File propertiesFile = saveFolder.Combine(this.propertiesFile);
			if (!propertiesFile.Exists) {
				Logger.LogError("Properties file not found: " + propertiesFile.FullPath);
				return null;
			}
			try {
				JObject json = JObject.Parse(propertiesFile.ReadAllText());
				DataResult<LevelPropertyInfo> infoResult = LevelPropertyInfo.CODEC.Decode(json);
				LevelPropertyInfo info = infoResult.GetOrThrow(m => new InvalidOperationException("Failed to read level properties: " + m));

				File chunksFolder = saveFolder.Combine(this.chunksFolder);
				chunksFolder.Mkdir();

				return new WorldSave(saveFolder, chunksFolder, saveFolder.Name, info);
			} catch (Exception e) {
				Logger.LogFatal(e);
				saveFolder.Delete();
				return null;
			}
		}

	}
}
