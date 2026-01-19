using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public static class EntityDescriptorRegistry {
		static readonly Dictionary<string, EntityDescriptor> byId = new();
		static readonly Dictionary<Type, EntityDescriptor> byType = new();

		private static readonly EntityDescriptor player = Register(
			new PrefabEntityDescriptor("entity.player", "Player", new AssetKey("player"), ResourceManager.GetRuntimePrefab), 
			typeof(PlayerController)
		);
		private static readonly EntityDescriptor droppedItem = Register(
			new PrefabEntityDescriptor("entity.droppedItem", nameof(DroppedItem), new AssetKey("droppedItem")),
			typeof(DroppedItem)
		);
		
		private static EntityDescriptor Register(EntityDescriptor descriptor, Type type = null) {
			byId[descriptor.ID] = descriptor;
			if (type != null) {
				byType[type] = descriptor;
			}
			return descriptor;
		}

		public static EntityDescriptor ByID(string id) {
			return byId[id];
		}

		public static EntityDescriptor ByType(Type type) {
			return byType[type];
		}

		public static EntityDescriptor ByType<T>() {
			return byType[typeof(T)];
		}
	}
}
