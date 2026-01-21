using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using UnityEngine;

namespace SoulboundBackend.Client.ItemSystem {
	public static class WorldPrefabFactory {
		//public static readonly GameObject worldPrefab = ResourceManager.Get<GameObject, ResourceGroups.Prefabs>(new AssetKey("droppedItem"));
		public static readonly GameObject worldPrefab = ResourceManager.GetAddressableSync<GameObject>(new AssetKey("droppedItem"));

		public static Func<GameObject> GetInstantiator() {
			return () => GameObject.Instantiate(worldPrefab);
		}
	}
}
