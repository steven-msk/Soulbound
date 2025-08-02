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

	public BlockState Place(ItemStack itemStack, BlockPos position) {
		itemStack.Quantity--;
		return new BlockState(referenceBlock);
	}

	protected override CompoundTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(Tooltip.Info(this.itemName));
	}

	public static BlockItem FromBlock(Block block) {
		List<BlockItem> allBlockItems = AssetRegistry.GetAll<BlockItem>();
		return allBlockItems.FirstOrDefault(item => item.referenceBlock == block) ?? null;
    }
}
