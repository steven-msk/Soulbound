using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core.Assets;
using System;
using UnityEngine;

namespace SoulboundBackend.Client.ItemSystem.View {
	public record ItemAspect {
		private static readonly AssetKey droppedItemKey = new("droppedItem");
		public ItemIcon icon { get; private set; }
		public Func<GameObject> worldPrefabSupplier { get; private set; }

		public ItemAspect(ItemIcon icon, Func<GameObject> worldPrefabSupplier) {
			this.icon = icon;
			this.worldPrefabSupplier = worldPrefabSupplier;
		}

		public static ItemAspect Simple(AssetKey spriteKey, int ppu = 100) {
			ItemIcon icon = new(spriteKey, ppu);
			return new ItemAspect(icon, () => GameObject.Instantiate(AssetManager.Resolve<GameObject>(droppedItemKey)));
		}
	}
}
