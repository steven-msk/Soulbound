using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public record ItemAspect {
	public ItemIcon icon { get; private set; }
	public Func<GameObject> worldPrefabSupplier { get; private set; }

	public ItemAspect(ItemIcon icon, Func<GameObject> worldPrefabSupplier) {
		this.icon = icon;
		this.worldPrefabSupplier = worldPrefabSupplier;
	}

	public static ItemAspect Simple(string iconSpriteName, int ppu = 100) {
		ItemIcon icon = new(GetIconSprite(iconSpriteName), ppu);
		return new ItemAspect(icon, WorldPrefabFactory.FromIcon(icon));
	}

	public static Sprite GetIconSprite(string spriteName) {
		return ResourceManager.Get<Sprite, ResourceGroups.Items.Icons>(spriteName);
	}
}
