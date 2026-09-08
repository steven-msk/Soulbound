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
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Widget;
	using System;
	using System.Linq;

	public static class Registries {
		private static bool freezed = false;
		public static readonly Identifier ROOT_IDENTIFIER = Identifier.Of("root");
		public static readonly Registry<IRegistry> ROOT = CreateRoot(ROOT_IDENTIFIER);

		public static readonly Registry<Block> BLOCKS = Create<Block>(Identifier.Of("block"));
		public static readonly Registry<Item> ITEMS = Create<Item>(Identifier.Of("item"));
		public static readonly Registry<EntityDescriptor> ENTITIES = Create<EntityDescriptor>(Identifier.Of("entity"));
		public static readonly Registry<AttributeType> ATTRIBUTE = Create<AttributeType>(Identifier.Of("attribute"));
		public static readonly Registry<TileEntityType> TILE_ENTITIES = Create<TileEntityType>(Identifier.Of("tile_entity"));
		public static readonly Registry<InventoryScreenHandlerType> INVENTORY_SCREEN_HANDLES = Create<InventoryScreenHandlerType>(Identifier.Of("inventory_screen_handle"));
		public static readonly Registry<RecipeType> RECIPE_TYPE = Create<RecipeType>(Identifier.Of("recipe_type"));
		public static readonly Registry<ComponentType> COMPONENT_TYPE = Create<ComponentType>(Identifier.Of("component_type"));
		public static readonly Registry<WorldWidgetType> WORLD_WIDGET_TYPE = Create<WorldWidgetType>(Identifier.Of("world_widget"));
		public static readonly Registry<WorldPreset> WORLD_PRESET = Create<WorldPreset>(Identifier.Of("world_preset"));
		public static readonly Registry<LevelType> LEVEL_TYPE = Create<LevelType>(Identifier.Of("level_type"));
		public static readonly Registry<MapCodec<ChunkGenerator>> CHUNK_GENERATOR = Create<MapCodec<ChunkGenerator>>(Identifier.Of("chunk_generator"));
		public static readonly Registry<LevelSettings> LEVEL_SETTINGS = Create<LevelSettings>(Identifier.Of("level_settings"));

		// temporary, see LootTables
		public static readonly Registry<LootTable> LOOT_TABLES = Create<LootTable>(Identifier.Of("loot_table"));

		private static Registry<T> Create<T>(Identifier id) {
			if (freezed) throw new InvalidOperationException("Registries already freezed");

			RegistryKey<Registry<T>> registryKey = RegistryKey<T>.OfRegistry(id);
			Registry<T> registry = Registry<IRegistry>.RegisterVariant(ROOT, registryKey, new Registry<T>(registryKey));

			return registry;
		}

		private static Registry<IRegistry> CreateRoot(Identifier identifier) {
			return new Registry<IRegistry>(RegistryKey<IRegistry>.OfRegistry(identifier));
		}

		public static void Init() {
			Blocks.Init();
			Items.Init();
			EntityType.Init();
			Attributes.Init();
			TileEntityType.Init();
			InventoryScreenHandlerType.Init();
			RecipeType.Init();
			LootTables.Init();
			WorldWidgetType.Init();
			WorldPreset.Init();
			LevelType.Init();
			LevelSettings.Init();
			ChunkGenerators.Init();
		}

		public static void Freeze() {
			freezed = true;
			Logger.LogInfo("Freezing {} registries", ROOT.Count());

			foreach (IRegistry registry in ROOT) {
				registry.Freeze();
			}
		}
	}
}
