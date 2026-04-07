using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public static class EntityType {
		public static readonly EntityDescriptor MOVING_ENTITY = Register(new EntityDescriptor(Identifier.Of("moving_entity"), pos => new MovingEntity(pos)));
		public static readonly EntityDescriptor STATIC_ENTITY = Register(new EntityDescriptor(Identifier.Of("static_entity"), pos => new StaticEntity(pos)));
		public static readonly EntityDescriptor PHYSICS_ENTITY = Register(new EntityDescriptor(Identifier.Of("physics_entity"), pos => new PhysicsEntity(pos)));
		public static readonly EntityDescriptor AREA_TRIGGER_ENTITY = Register(new EntityDescriptor(Identifier.Of("area_trigger_entity"), pos => new AreaTriggerEntity(pos)));

		private static TEntity Register<TEntity>(TEntity descriptor) where TEntity : EntityDescriptor {
			return Registries.Register(Registries.ENTITIES, descriptor.GetIdentifier(), descriptor);
		}


		public static void Init() { }
	}
}
