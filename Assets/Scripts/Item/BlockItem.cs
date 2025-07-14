using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Items/BlockItem")]
public class BlockItem : Item, IPlaceable {
	[SerializeField] private Block referenceBlock;

	public Block Place(ItemStack itemStack, Vector2Int position, Tilemap tilemap) {
		referenceBlock.Place(position, tilemap);
		itemStack.Quantity--;
		return referenceBlock;
	}

	protected override CompoundTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(Tooltip.Info(this.itemName));
	}
}
