namespace SoulboundEngine.Registry {
	using SoulboundEngine.Component;
	using SoulboundEngine.Inventory;
	using SoulboundEngine.Item;
	using SoulboundEngine.Loot;
	using SoulboundEngine.Recipe;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Entity.Attribute;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Gen.Function;
	using SoulboundEngine.World.Gen.Noise;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Widget;

	public static class RegistryKeys {
		public static readonly RegistryKey<Registry<Block>> BLOCK = Create<Block>("block");
		public static readonly RegistryKey<Registry<Item>> ITEM = Create<Item>("item");
		public static readonly RegistryKey<Registry<EntityDescriptor>> ENTITY = Create<EntityDescriptor>("entity");
		public static readonly RegistryKey<Registry<AttributeType>> ATTRIBUTE = Create<AttributeType>("attribute");
		public static readonly RegistryKey<Registry<TileEntityType>> TILE_ENTITY = Create<TileEntityType>("tile_entity");
		public static readonly RegistryKey<Registry<InventoryScreenHandlerType>> INVENTORY_SCREEN_HANDLER = Create<InventoryScreenHandlerType>("inventory_screen_handler");
		public static readonly RegistryKey<Registry<RecipeType>> RECIPE_TYPE = Create<RecipeType>("recipe_type");
		public static readonly RegistryKey<Registry<LootTable>> LOOT_TABLE = Create<LootTable>("loot_table");
		public static readonly RegistryKey<Registry<ComponentType>> COMPONENT_TYPE = Create<ComponentType>("component_type");
		public static readonly RegistryKey<Registry<WorldWidgetType>> WORLD_WIDGET = Create<WorldWidgetType>("world_widget");
		public static readonly RegistryKey<Registry<WorldPreset>> WORLD_PRESET = Create<WorldPreset>("world_preset");
		public static readonly RegistryKey<Registry<LevelType>> LEVEL_TYPE = Create<LevelType>("level_type");
		public static readonly RegistryKey<Registry<LevelSettings>> LEVEL_SETTINGS = Create<LevelSettings>("level_settings");
		public static readonly RegistryKey<Registry<MapCodec<ChunkGenerator>>> CHUNK_GENERATOR = Create<MapCodec<ChunkGenerator>>("chunk_generator");
		public static readonly RegistryKey<Registry<Biome>> BIOME = Create<Biome>("biome");
		public static readonly RegistryKey<Registry<MapCodec<BiomeSource>>> BIOME_SOURCE = Create<MapCodec<BiomeSource>>("biome_source");
		public static readonly RegistryKey<Registry<NormalNoise.Parameters>> NOISE = Create<NormalNoise.Parameters>("noise");
		public static readonly RegistryKey<Registry<NoiseGeneratorSettings>> NOISE_SETTINGS = Create<NoiseGeneratorSettings>("noise_settings");
		public static readonly RegistryKey<Registry<IDensityFunction>> DENSITY_FUNCTION = Create<IDensityFunction>("density_function");
		public static readonly RegistryKey<Registry<MultiNoiseBiomeSourceParamList>> MULTI_NOISE_BIOME_SOURCE_PARAMETER_LIST = Create<MultiNoiseBiomeSourceParamList>(
			"multi_noise_biome_source_parameter_list"
		);

		private static RegistryKey<Registry<T>> Create<T>(string id) where T : class {
			return RegistryKey<T>.OfRegistry(Identifier.Of(id));
		}
	}
}
