using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core.Resource;
using System;
using UnityEngine;

namespace SoulboundBackend.Client.ItemSystem {
	public static class WorldPrefabFactory {
		public static readonly GameObject worldPrefabReference = ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("droppedItem");

		public static Func<GameObject> FromIcon(ItemIcon icon) {
			return () => {
				GameObject worldPrefab = CreateNewInstance();
				DroppedItem droppedItem = worldPrefab.GetComponent<DroppedItem>() ?? worldPrefab.AddComponent<DroppedItem>();
				//droppedItem.ApplyIcon(icon);
				return worldPrefab;
			};
		}

		public static Func<GameObject> FromIcon(Func<ItemIcon> iconSupplier) {
			return FromIcon(iconSupplier.Invoke());
		}

		/// <summary>
		/// Should only be used in the context of WorldPrefabFactory.
		/// Any external usage should be made carefully.
		/// </summary>
		/// <returns>A raw instance of the world prefab</returns>
		public static GameObject CreateNewInstance() {
			return GameObject.Instantiate(worldPrefabReference);
		}
	}
}
