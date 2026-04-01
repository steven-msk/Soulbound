using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.EntitySystem {
	public static class EntityType {
		public static readonly EntityDescriptor MOVING_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(new Identifier("movingEntity"), pos => new MovingEntity(pos)));
		public static readonly EntityDescriptor STATIC_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(new Identifier("staticEntity"), pos => new StaticEntity(pos)));
		public static readonly EntityDescriptor PHYSICS_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(new Identifier("physicsEntity"), pos => new PhysicsEntity(pos)));
		public static readonly EntityDescriptor AREA_TRIGGER_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(new Identifier("areaTriggerEntity"), pos => new AreaTriggerEntity(pos)));
	}
}
