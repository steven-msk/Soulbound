using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public static class EntityType {
		// contract initialization, do not modify
		private static readonly IRegistrationContract<EntityDescriptor, IRegistrationKey<EntityDescriptor>> _contract = Registry<EntityDescriptor>.SetContract(new EntityDescriptorRegistrationContract());

		[PROTOTYPICAL]
		public static readonly EntityDescriptor MOVING_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor("movingEntity", pos => new MovingEntity(pos)));
		[PROTOTYPICAL]
		public static readonly EntityDescriptor STATIC_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor("staticEntity", pos => new StaticEntity(pos)));
		[PROTOTYPICAL]
		public static readonly EntityDescriptor PHYSICS_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor("physicsEntity", pos => new PhysicsEntity(pos)));
		[PROTOTYPICAL]
		public static readonly EntityDescriptor AREA_TRIGGER_ENTITY = Registry<EntityDescriptor>.Add(new EntityDescriptor("areaTriggerEntity", pos => new AreaTriggerEntity(pos)));

		public sealed class EntityDescriptorRegistrationContract : IRegistrationContract<EntityDescriptor, IRegistrationKey<EntityDescriptor>> {
			public IRegistrationKey<EntityDescriptor> ValueToKey(EntityDescriptor value) {
				return new EntityDescriptor.RegistrationKey(value.GetID());
			}
		}
	}
}
