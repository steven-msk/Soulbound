using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public static class EntityType {
		public static readonly EntityDescriptor MOVING_ENTITY = Register(Identifier.Of("moving_entity"), new EntityDescriptor((_, level) => new MovingEntity(level)));
		public static readonly EntityDescriptor STATIC_ENTITY = Register(Identifier.Of("static_entity"), new EntityDescriptor((_, level) => new StaticEntity(level)));
		public static readonly EntityDescriptor PHYSICS_ENTITY = Register(Identifier.Of("physics_entity"), new EntityDescriptor((_, level) => new PhysicsEntity(level)));
		public static readonly EntityDescriptor AREA_TRIGGER_ENTITY = Register(Identifier.Of("area_trigger_entity"), new EntityDescriptor((_, level) => new AreaTriggerEntity(level)));

		private static TEntity Register<TEntity>(Identifier id, TEntity descriptor) where TEntity : EntityDescriptor {
			return Registries.Register(Registries.ENTITIES, id, descriptor);
		}


		public static void Init() { }
	}
}
