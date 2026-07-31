using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

// TEMPORARY IMPLEMENTATION
// USED ONLY FOR SERIALIZATION CONTRACT VALIDATION

namespace SoulboundEngine.Client.World.Block.Entity {
	public static class TileEntitySerializer {
		private const string PATH = "cachedTileEntities.json";

		public static void WriteAll(IEnumerable<TileEntity> tileEntities) {
			JArray objArray = new();
			foreach (var tileEntity in tileEntities) {
				JObject obj = Write(tileEntity);
				objArray.Add(obj);
			}
			string json = JsonConvert.SerializeObject(objArray, new JsonSerializerSettings() { Formatting = Formatting.Indented });
			File.WriteAllText(GetFilePath(), json);
		}

		public static IEnumerable<TileEntity> ReadAll() {
			List<TileEntity> tileEntities = new();
			try {
				string json = File.ReadAllText(GetFilePath());
				JArray objArray = JArray.Parse(json);

				foreach (var token in objArray) {
					TileEntity entity = Read(token);
					tileEntities.Add(entity);
				}

				return tileEntities;
			} catch (FileNotFoundException e) {
				Logger.LogError(e);
				return Enumerable.Empty<TileEntity>().ToList();
			}
		}

		public static JObject Write(TileEntity entity) {
			JObject json = new() {
				["type"] = TileEntityType.GetId(entity.GetTileEntityType())!.ToString(),
				["pos"] = entity.blockPos.ToString(),
			};
			entity.Write(json);
			return json;
		}

		public static TileEntity Read(JToken json) {
			if (!Identifier.TryParse((string)json["type"], out Identifier typeId)) {
				Logger.LogError("Failed to read tile entity type id");
				return null;
			}
			TileEntityType type = Registries.TILE_ENTITIES.Get(typeId);

			BlockPos blockPos = BlockPos.Parse((string)json["pos"]);
			// blockState will be parsed from world deserialization
			BlockState state = Blocks.AIR.DefaultState;

			TileEntity tileEntity = type.Instantiate(blockPos, state);
			tileEntity.Read(json);
			return tileEntity;
		}

		private static string GetFilePath() {
			return Path.Combine(Application.persistentDataPath, PATH);
		}
	}
}
