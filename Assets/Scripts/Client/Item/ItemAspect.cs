using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using UnityEngine;

public record ItemAspect {
	public ItemIcon icon { get; private set; }
	public Func<GameObject> worldPrefabSupplier { get; private set; }

	public ItemAspect(ItemIcon icon, Func<GameObject> worldPrefabSupplier) {
		this.icon = icon;
		this.worldPrefabSupplier = worldPrefabSupplier;
	}

	public static ItemAspect Simple(AssetKey spriteKey, int ppu = 100) {
		ItemIcon icon = new(spriteKey, ppu);
		return new ItemAspect(icon, WorldPrefabFactory.GetInstantiator());
	}
}
