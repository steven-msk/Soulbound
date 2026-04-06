using SoulboundEngine.Client.World.EntitySystem.Attribute;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public static class EntityType {
		public static readonly EntityDescriptor MOVING_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(Identifier.Of("moving_entity"), pos => new MovingEntity(pos)));
		public static readonly EntityDescriptor STATIC_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(Identifier.Of("static_entity"), pos => new StaticEntity(pos)));
		public static readonly EntityDescriptor PHYSICS_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(Identifier.Of("physics_entity"), pos => new PhysicsEntity(pos)));
		public static readonly EntityDescriptor AREA_TRIGGER_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(Identifier.Of("area_trigger_entity"), pos => new AreaTriggerEntity(pos)));

		public static readonly EntityDescriptor ATTRIBUTE_ENTITY = Registry<EntityDescriptor>.Add(
			new EntityDescriptor(
				identifier: Identifier.Of("attribute_entity"),
				pos => new StaticEntity(pos),
				new Dictionary<AttributeType, object> {
					[Attributes.intType] = 1,
				},
				new Dictionary<AttributeType, IValueRule?> {
					[Attributes.intType] = new NumberRange(-100, 100),
				}
			)
		);
	}
}
