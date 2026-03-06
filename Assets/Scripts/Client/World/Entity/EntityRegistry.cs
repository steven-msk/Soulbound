using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public static class EntityRegistry {
		private static readonly Dictionary<string, EntityDescriptor> descriptors = new();

		public static EntityDescriptor Register(EntityDescriptor descriptor) {
			descriptors[descriptor.id] = descriptor;
			return descriptor;
		}

		public static bool TryGet(string id, out EntityDescriptor descriptor) {
			return descriptors.TryGetValue(id, out descriptor);
		}

		public static IEnumerable<EntityDescriptor> GetAll() => descriptors.Values;
	}
}
