using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Client.World.EntitySystem.Attribute;
using System;
using System.Linq;

namespace SoulboundEngine.Core.Registry {
	public static class Registries {
		private static bool freezed = false;
		public static readonly Identifier ROOT_IDENTIFIER = Identifier.Of("root");
		public static readonly Registry<IRegistry> ROOT = CreateRoot(ROOT_IDENTIFIER);

		public static readonly Registry<Block> BLOCKS = Create<Block>(Identifier.Of("block"));
		public static readonly Registry<Item> ITEMS = Create<Item>(Identifier.Of("item"));
		public static readonly Registry<EntityDescriptor> ENTITIES = Create<EntityDescriptor>(Identifier.Of("entity"));
		public static readonly Registry<EntityAttribute> ATTRIBUTES = Create<EntityAttribute>(Identifier.Of("attribute"));
		public static readonly Registry<TileEntityType> TILE_ENTITIES = Create<TileEntityType>(Identifier.Of("tile_entity")); 

		private static Registry<T> Create<T>(Identifier id) {
			if (freezed) throw new InvalidOperationException("Registries already freezed");

			RegistryKey<Registry<T>> registryKey = RegistryKey<T>.OfRegistry(id);
			Registry<T> registry = Registry<IRegistry>.Register(ROOT, registryKey, new Registry<T>(registryKey));

			return registry;
		}

		private static Registry<IRegistry> CreateRoot(Identifier identifier) {
			return new Registry<IRegistry>(RegistryKey<IRegistry>.OfRegistry(identifier));
		}

		public static void Init() {
			Blocks.Init();
			Items.Init();
			EntityType.Init();
			AttributeTypes.Init();
			TileEntityTypes.Init();
		}

		public static void Freeze() {
			freezed = true;
			Logger.LogInfo("Freezing {} registries", ROOT.Count());

			foreach (var registry in ROOT) {
				registry.Freeze();
			}
		}
	}
}
