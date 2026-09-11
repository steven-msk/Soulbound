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
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Widget;
	using System;
	using System.Collections.Generic;

	public static class Registries {
		private delegate object RegistryBootstrapper<T>(Registry<T> registry) where T : class;
		private static readonly Dictionary<Identifier, Func<object>> LOADERS = new();
		private static bool freezed = false;
		public static readonly Identifier ROOT_IDENTIFIER = Identifier.Of("root");
		public static readonly Registry<IRegistry> ROOT = new(RegistryKey<IRegistry>.OfRegistry(ROOT_IDENTIFIER));
		public static readonly Registry<Block> BLOCK = Create(RegistryKeys.BLOCK, Blocks.Init);
		public static readonly Registry<Item> ITEM = Create(RegistryKeys.ITEM, Items.Init);
		public static readonly Registry<EntityDescriptor> ENTITY = Create(RegistryKeys.ENTITY, EntityType.Init);
		public static readonly Registry<AttributeType> ATTRIBUTE = Create(RegistryKeys.ATTRIBUTE, Attributes.Init);
		public static readonly Registry<TileEntityType> TILE_ENTITIES = Create(RegistryKeys.TILE_ENTITY, TileEntityType.Init);
		public static readonly Registry<InventoryScreenHandlerType> INVENTORY_SCREEN_HANDLES = Create(RegistryKeys.INVENTORY_SCREEN_HANDLER, InventoryScreenHandlerType.Init);
		public static readonly Registry<RecipeType> RECIPE_TYPE = Create(RegistryKeys.RECIPE_TYPE, RecipeType.Init);
		public static readonly Registry<ComponentType> COMPONENT_TYPE = Create(RegistryKeys.COMPONENT_TYPE, r => ItemComponents.DEFAULT_COMPONENTS);
		public static readonly Registry<WorldWidgetType> WORLD_WIDGET_TYPE = Create(RegistryKeys.WORLD_WIDGET, WorldWidgetType.Init);
		public static readonly Registry<MapCodec<ChunkGenerator>> CHUNK_GENERATOR = Create(RegistryKeys.CHUNK_GENERATOR, ChunkGenerators.Init);
		// temporary, see LootTables
		public static readonly Registry<LootTable> LOOT_TABLES = Create(RegistryKeys.LOOT_TABLE, LootTables.Init);
		// not definitive
		public static readonly Registry<LevelType> LEVEL_TYPE = Create(RegistryKeys.LEVEL_TYPE, LevelType.Init);
		public static readonly Registry<WorldPreset> WORLD_PRESET = Create(RegistryKeys.WORLD_PRESET, WorldPreset.Init);
		public static readonly Registry<Biome> BIOME = Create(RegistryKeys.BIOME, Biome.Init);
		public static readonly Registry<NormalNoise.Parameters> NOISE = Create(RegistryKeys.NOISE, NoiseData.Init);

		private static Registry<T> Create<T>(RegistryKey<Registry<T>> key, RegistryBootstrapper<T> bootstrapper) where T : class {
			return Register(key, new Registry<T>(key), bootstrapper);
		}

		private static Registry<T> Register<T>(RegistryKey<Registry<T>> key, Registry<T> registry, RegistryBootstrapper<T> bootstrapper) where T : class {
			if (freezed) throw new InvalidOperationException("Registries already freezed");
			Identifier id = key.value;
			LOADERS.Add(id, () => bootstrapper(registry));
			return Registry<IRegistry>.RegisterVariant(ROOT, key, registry);
		}

		public static void Init() {
			AddContents();
			Freeze();
			Validate(ROOT);
		}

		private static void AddContents() {
			foreach ((Identifier id, Func<object> loader) in LOADERS) {
				if (loader() == null) {
					Logger.LogError("Unable to load registry '{}'", id);
				}
			}
		}

		public static void Freeze() {
			Logger.LogInfo("Freezing registries");
			if (freezed) throw new InvalidOperationException("Registries already freezed");
			freezed = true;
			ROOT.Freeze();

			int c = 0;
			foreach (IRegistry registry in ROOT) {
				registry.Freeze();
				c++;
			}
			Logger.LogInfo("Freezed {} registries", c);
		}

		private static void Validate(Registry<IRegistry> registry) {
			foreach (IRegistry r in registry) {
				if (r.GetIdentifiers().Count == 0) {
					Logger.LogError("Registry '{}' was empty after loading", registry.GetKey(r));
				}
			}
		}
	}
}
