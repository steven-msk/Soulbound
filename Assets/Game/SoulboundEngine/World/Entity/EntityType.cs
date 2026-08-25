namespace SoulboundEngine.World.Entity {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Player;

#nullable enable

	public static class EntityType {
		public static readonly EntityDescriptor<PlayerEntity> PLAYER = Register("player", EntityDescriptor<PlayerEntity>.Builder.OfNothing(EntityCategory.PLAYER));
		public static readonly EntityDescriptor<ItemEntity> ITEM = Register("item", 
			EntityDescriptor<ItemEntity>.Builder.Of(EntityCategory.OTHER, ItemEntity.Create)
				.Sized(1.0d, 1.0d)
		);

		//public static readonly EntityDescriptor<MovingEntity> MOVING_ENTITY = Register(
		//	"moving_entity",
		//	(descriptor, level) => new MovingEntity(descriptor, level),
		//	ITransformSupplier<MovingEntity>.Of(entity => {
		//		GameObject obj = new("Static Entity", typeof(StaticTransform));

		//		Sprite sprite = AssetManager.Resolve<Sprite>(new AssetKey("WhiteSquare"));
		//		obj.AddComponent<SpriteRenderer>().sprite = sprite;

		//		return obj.GetComponent<StaticTransform>();
		//	})
		//);
		//public static readonly EntityDescriptor<StaticEntity> STATIC_ENTITY = Register(
		//	"static_entity",
		//	(descriptor, level) => new StaticEntity(descriptor, level),
		//	ITransformSupplier<StaticEntity>.Of(entity => {
		//		GameObject obj = new("Static Entity", typeof(StaticTransform));

		//		Sprite sprite = AssetManager.Resolve<Sprite>(new AssetKey("WhiteSquare"));
		//		obj.AddComponent<SpriteRenderer>().sprite = sprite;

		//		return obj.GetComponent<StaticTransform>();
		//	})
		//);
		//public static readonly EntityDescriptor<AreaTriggerEntity> AREA_TRIGGER_ENTITY = Register(
		//	"area_trigger_entity",
		//	(descriptor, level) => new AreaTriggerEntity(descriptor, level),
		//	ITransformSupplier<AreaTriggerEntity>.Of(entity => {
		//		GameObject obj = new("Area Trigger Entity");

		//		BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
		//		collider.size = new Vector2(4f, 0.15f);
		//		collider.isTrigger = true;
		//		collider.excludeLayers = ~LayerMask.GetMask(Layers.EntityCharacter);

		//		TriggerCollisionListener triggerListener = obj.AddComponent<TriggerCollisionListener>();
		//		triggerListener.onTriggerEnter += entity.OnAreaEnter;
		//		triggerListener.onTriggerExit += entity.OnAreaExit;

		//		return obj.AddComponent<StaticTransform>();
		//	})
		//);

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
