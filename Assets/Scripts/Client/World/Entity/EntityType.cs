using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public static class EntityType {
		[PROTOTYPICAL]
		public static readonly EntityDescriptor MOVING_ENTITY = EntityRegistry.Register(new("movingEntity", pos => new MovingEntity(pos)));
		[PROTOTYPICAL]
		public static readonly EntityDescriptor STATIC_ENTITY = EntityRegistry.Register(new("staticEntity", pos => new StaticEntity(pos)));
	}
}
