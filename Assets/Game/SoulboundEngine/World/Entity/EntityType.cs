namespace SoulboundEngine.World.Entity {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Player;

#nullable enable

	public static class EntityType {
		public static readonly EntityDescriptor<PlayerEntity> PLAYER = Register("player", 
			EntityDescriptor<PlayerEntity>.Builder.OfNothing(
				EntityCategory.PLAYER, PlayerEntity.CreateDefaultAttributes().Build()
			).CannotSpawnByCommand()
		);
		public static readonly EntityDescriptor<ItemEntity> ITEM = Register("item", 
			EntityDescriptor<ItemEntity>.Builder.Of(
				EntityCategory.OTHER, ItemEntity.Create, ItemEntity.CreateDefaultAttributes().Build()
			).CannotSpawnByCommand()
			.Sized(1.0d, 1.0d)
		);

		private static EntityDescriptor<E> Register<E>(string id, EntityDescriptor<E>.Builder builder) where E : Entity {
			return Register(KeyOf(id), builder);
		}

		private static EntityDescriptor<E> Register<E>(RegistryKey<EntityDescriptor> key, EntityDescriptor<E>.Builder builder) where E : Entity {
			return Register(key, builder.Build());
		}

		private static EntityDescriptor<E> Register<E>(RegistryKey<EntityDescriptor> key, EntityDescriptor<E> descriptor) where E : Entity {
			return Registry<EntityDescriptor>.Register(Registries.ENTITIES, key, descriptor);
		}

		private static RegistryKey<EntityDescriptor> KeyOf(string id) {
			return RegistryKey<EntityDescriptor>.Of(Registries.ENTITIES.GetKey(), Identifier.Of(id));
		}

		public static void Init() { }
	}
}
