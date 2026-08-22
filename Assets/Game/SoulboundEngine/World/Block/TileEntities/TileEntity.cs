namespace SoulboundEngine.World.Block.Entity {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Level;

#nullable enable

	public abstract class TileEntity {
		protected Level? level;
		public readonly BlockPos blockPos;
		private readonly TileEntityType tileEntityType;
		private BlockState blockState;

		public TileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState) {
			this.tileEntityType = tileEntityType;
			this.blockPos = blockPos;
			this.blockState = blockState;
		}

		public virtual void OnDispose() { }

		public TileEntityType GetTileEntityType() => this.tileEntityType;

		public void SetLevel(Level? level) => this.level = level;
		public Level? GetLevel() => this.level;

		public BlockPos GetBlockPos() => this.blockPos;

		public BlockState GetBlockState() => this.blockState;
		public void SetBlockState(BlockState blockState) => this.blockState = blockState;

		public bool HasLevel() => this.level != null;

		public JToken Write() {
			JObject json = new() {
				["type"] = TileEntityType.GetId(this.tileEntityType)!.ToString(),
				["x"] = this.blockPos.x,
				["y"] = this.blockPos.y,
			};
			this.WriteAdditional(json);
			return json;
		}

		public static TileEntity? FromJson(JToken json, BlockPos pos, BlockState blockState) {
			if (json.Type != JTokenType.Object) {
				Logger.LogError("TileEntity json is not object: {}", json);
				return null;
			}

			string? typeIdString = (string?)json["type"];
			if (typeIdString == null) {
				Logger.LogError("No type property on tile entity json: {}", json);
				return null;
			}
			if (!Identifier.TryParse(typeIdString, out Identifier typeId)) {
				Logger.LogError("Could not parse tile entity type id: {}", typeIdString);
				return null;
			}
			RegistryEntry<TileEntityType>? entry = Registries.TILE_ENTITIES.GetEntry(typeId);
			if (entry == null) {
				Logger.LogError("Could not find entity type in registry: {}", typeIdString);
				return null;
			}

			TileEntityType type = entry.GetValue();
			TileEntity tileEntity = type.Instantiate(pos, blockState);
			tileEntity.ReadAdditional((JObject)json);
			return tileEntity;
		}

		public static BlockPos? GetPosFromJson(JToken json) {
			if (json.Type != JTokenType.Object) {
				Logger.LogError("TileEntity json is not object: {}", json);
				return null;
			}
			int? x = (int?)json["x"];
			if (x == null) {
				Logger.LogError("No x property on tile entity json: {}", json);
				return null;
			}
			int? y = (int?)json["y"];
			if (y == null) {
				Logger.LogError("No y property on tile entity json: {}", json);
				return null;
			}
			return new BlockPos(x.Value, y.Value);
		}

		public virtual void WriteAdditional(JObject json) {
		}

		public virtual void ReadAdditional(JObject json) {
		}

		public bool IsValidBlockState(BlockState blockState) {
			return this.GetTileEntityType().Supports(blockState);
		}

		public override string ToString() => this.GetTileEntityType().ToString();
	}
}
