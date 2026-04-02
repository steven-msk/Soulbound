using SoulboundEngine.Client.World.EntitySystem.Attribute;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public static class EntityType {
		public static readonly EntityDescriptor MOVING_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(new Identifier("movingEntity"), pos => new MovingEntity(pos)));
		public static readonly EntityDescriptor STATIC_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(new Identifier("staticEntity"), pos => new StaticEntity(pos)));
		public static readonly EntityDescriptor PHYSICS_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(new Identifier("physicsEntity"), pos => new PhysicsEntity(pos)));
		public static readonly EntityDescriptor AREA_TRIGGER_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor(new Identifier("areaTriggerEntity"), pos => new AreaTriggerEntity(pos)));

		public static readonly EntityDescriptor ATTRIBUTE_ENTITY = Registry<EntityDescriptor>.Add(
			new EntityDescriptor(
				identifier: new Identifier("attributeEntity"),
				pos => new StaticEntity(pos),
				new Dictionary<AttributeType, object> {
					[Attributes.intType] = 1,
					[Attributes.stringType] = "string1"
				},
				new Dictionary<AttributeType, IValueRule?> {
					[Attributes.intType] = new NumberRange<int>(0, 100),
					[Attributes.stringType] = new SetValidator<string>(new[] { "string1", "string2", "string3", "string4", "string5" })
				}
			)
		);
	}
}
