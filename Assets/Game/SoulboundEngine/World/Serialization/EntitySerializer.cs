namespace SoulboundEngine.World.Serialization {
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Client.Debug.Logging;
	using SoulboundEngine.Client.World;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;

#nullable enable

	public class EntitySerializer {
		private const string ENTITIES_FILE = "entities.json";
		private const string PLAYER_FILE = "playerData.json";
		private readonly WorldSave save;

		public EntitySerializer(WorldSave save) {
			this.save = save;
		}

		public IEnumerable<Entity> LoadAll(Level level) {
			if (this.save.isNew) yield break;

			File entitiesFile = ToEntitiesFile(this.save.saveFolder);
			if (!entitiesFile.Exists) yield break;

			string jsonText = entitiesFile.ReadAllText();
			JArray array;
			try {
				array = JArray.Parse(jsonText);
			} catch (Exception e) {
				Logger.LogFatal(e, "Could not parse entities json object at {}", entitiesFile.FullPath);
				yield break;
			}

			foreach (JToken token in array) {
				Entity? entity = Entity.Load(token, level);
				if (entity != null) yield return entity;
			}
		}

		public void SaveAll(IEnumerable<Entity> entities) {
			File entitiesFile = ToEntitiesFile(this.save.saveFolder);
			entitiesFile.CreateNewFile();

			JArray array = new();
			foreach (Entity entity in entities) {
				array.Add(entity.Save());
			}

			entitiesFile.WriteAllText(array.ToString(Formatting.Indented));
		}

		public void SavePlayer(PlayerEntity player) {
			File playerFile = ToPlayerFile(this.save.saveFolder);
			playerFile.CreateNewFile();

			JToken token = player.Save();
			playerFile.WriteAllText(token.ToString(Formatting.Indented));
		}

		public bool LoadPlayer(PlayerEntity player) {
			if (this.save.isNew) return false;

			File playerFile = ToPlayerFile(this.save.saveFolder);
			if (!playerFile.Exists) {
				Logger.LogError("No player data file found: {}", playerFile.FullPath);
				return false;
			}

			try {
				string jsonText = playerFile.ReadAllText();
				JObject json = JObject.Parse(jsonText);
				player.Load(json);
			} catch (Exception e) {
				Logger.LogFatal(e, "Failed to read player data");
				return false;
			}

			return true;
		}

		public static File ToEntitiesFile(File saveFolder) {
			return saveFolder.Combine(ENTITIES_FILE);
		}

		public static File ToPlayerFile(File saveFolder) {
			return saveFolder.Combine(PLAYER_FILE);
		}
	}
}
