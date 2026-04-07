using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Client.World.EntitySystem.Attribute;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Render.Animation;

namespace SoulboundEngine.Core.Registry {
	public static class Registries {
		public delegate void Initializer<T>(Registry<T> registry);
		public static readonly Identifier ROOT_IDENTIFIER = Identifier.Of("root");
		public static readonly Registry<IRegistry> ROOT = new(RegistryKey<IRegistry>.OfRegistry(ROOT_IDENTIFIER));

		public static readonly Registry<Block> BLOCKS = Create(RegistryKey<Block>.OfRegistry(Identifier.Of("block")), _ => { });
		public static readonly Registry<Item> ITEMS = Create(RegistryKey<Item>.OfRegistry(Identifier.Of("item")), _ => { });
		public static readonly Registry<EntityDescriptor> ENTITIES = Create(RegistryKey<EntityDescriptor>.OfRegistry(Identifier.Of("entity")), _ => { });
		public static readonly Registry<EntityAttribute> ATTRIBUTES = Create(RegistryKey<EntityAttribute>.OfRegistry(Identifier.Of("attribute")), _ => { });

		// placeholder
		[PROTOTYPICAL]
		public static readonly Registry<SpriteAnimation> SPRITE_ANIMATIONS = Create(RegistryKey<SpriteAnimation>.OfRegistry(Identifier.Of("sprite_animation")), _ => { });

		public static T Register<T>(Registry<T> registry, string id, T entry) {
			return Register<T, T>(registry, Identifier.Of(id), entry);
		}

		public static T Register<T>(Registry<T> registry, Identifier id, T entry) {
			return Register<T, T>(registry, id, entry);
		}

		public static T Register<V, T>(Registry<V> registry, Identifier id, T entry) where T : V {
			registry.CreateEntry(id, entry);
			return entry;
		}
		public static RegistryEntry<T> RegisterEntry<T>(Registry<T> registry, string id, T entry) {
			return RegisterEntry<T, T>(registry, Identifier.Of(id), entry);
		}

		public static RegistryEntry<T> RegisterEntry<T>(Registry<T> registry, Identifier id, T entry) {
			return RegisterEntry<T, T>(registry, id, entry);
		}

		public static RegistryEntry<V> RegisterEntry<V, T>(Registry<V> registry, Identifier id, T entry) where T : V {
			return registry.CreateEntry(id, entry);
		}

		private static Registry<T> Create<T>(RegistryKey<Registry<T>> key, Initializer<T> initializer) {
			Registry<T> registry = Register(ROOT, key.Value, new Registry<T>(key));

			initializer(registry);
			return registry;
		}


		public static void Init() {
			Blocks.Init();
			Items.Init();
			EntityType.Init();
			AttributeTypes.Init();
		}
	}
}
