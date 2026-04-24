using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public static class EntityType {
		public static readonly EntityDescriptor<MovingEntity> MOVING_ENTITY = Register<MovingEntity>("moving_entity", (_, level) => new MovingEntity(level));
		public static readonly EntityDescriptor<StaticEntity> STATIC_ENTITY = Register<StaticEntity>("static_entity", (_, level) => new StaticEntity(level));
		public static readonly EntityDescriptor<PhysicsEntity> PHYSICS_ENTITY = Register<PhysicsEntity>("physics_entity", (_, level) => new PhysicsEntity(level));
		public static readonly EntityDescriptor<AreaTriggerEntity> AREA_TRIGGER_ENTITY = Register<AreaTriggerEntity>("area_trigger_entity", (_, level) => new AreaTriggerEntity(level));

		private static EntityDescriptor<E> Register<E>(string id, EntityDescriptor<E> descriptor) where E : Entity {
			return Registries.Register(Registries.ENTITIES, Identifier.Of(id), descriptor);
		}

		private static EntityDescriptor<E> Register<E>(string id, EntityDescriptor<E>.EntityFactory factory) where E : Entity {
			return Register(id, EntityDescriptor.Of(factory));
		}

		public static void Init() { }
	}
}
