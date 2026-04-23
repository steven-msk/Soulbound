using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public static class EntityType {
		public static readonly EntityDescriptor MOVING_ENTITY = Register("moving_entity", new EntityDescriptor((_, level) => new MovingEntity(level)));
		public static readonly EntityDescriptor STATIC_ENTITY = Register("static_entity", new EntityDescriptor((_, level) => new StaticEntity(level)));
		public static readonly EntityDescriptor PHYSICS_ENTITY = Register("physics_entity", new EntityDescriptor((_, level) => new PhysicsEntity(level)));
		public static readonly EntityDescriptor AREA_TRIGGER_ENTITY = Register("area_trigger_entity", new EntityDescriptor((_, level) => new AreaTriggerEntity(level)));

		private static TEntity Register<TEntity>(string id, TEntity descriptor) where TEntity : EntityDescriptor {
			return Registries.Register(Registries.ENTITIES, Identifier.Of(id), descriptor);
		}


		public static void Init() { }
	}
}
