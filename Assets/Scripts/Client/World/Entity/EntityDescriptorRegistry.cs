using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public static class EntityDescriptorRegistry {
		static readonly Dictionary<string, EntityDescriptor_OLD> byId = new();
		static readonly Dictionary<Type, EntityDescriptor_OLD> byType = new();

		private static readonly EntityDescriptor_OLD player = Register(
			new PrefabEntityDescriptor("entity.player", "Player", new AssetKey("player"), AssetManager.Resolve<GameObject>), 
			typeof(PlayerController)
		);
		private static readonly EntityDescriptor_OLD droppedItem = Register(
			new PrefabEntityDescriptor("entity.droppedItem", nameof(DroppedItem), new AssetKey("droppedItem")),
			typeof(DroppedItem)
		);
		
		private static EntityDescriptor_OLD Register(EntityDescriptor_OLD descriptor, Type type = null) {
			byId[descriptor.ID] = descriptor;
			if (type != null) {
				byType[type] = descriptor;
			}
			return descriptor;
		}

		public static EntityDescriptor_OLD ByID(string id) {
			return byId[id];
		}

		public static EntityDescriptor_OLD ByType(Type type) {
			return byType[type];
		}

		public static EntityDescriptor_OLD ByType<T>() {
			return byType[typeof(T)];
		}
	}
}
