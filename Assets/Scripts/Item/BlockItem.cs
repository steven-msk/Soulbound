using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

public class BlockItem : ItemDefinition, IPlaceable {
	public Func<Block> blockGetter { get; }
    public Block referenceBlock => blockGetter() ?? throw new InvalidOperationException("Block reference is not yet initialized.");

    public BlockItem(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Block> blockGetter, Func<Item, AbstractTooltip?> tooltipSupplier)
		: base(name, icon, worldPrefabSupplier, maxStackSize, tooltipSupplier) {
        this.blockGetter = blockGetter;
    }

	// TODO: add constructors for all types which extend ItemDefinitions

    public BlockItem(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Block> blockGetter) 
		: this(name, icon, worldPrefabSupplier, maxStackSize, blockGetter, ItemTooltips.DefaultTitle()) { }

    public BlockState Place(ItemStack itemStack, BlockPos position) {
		itemStack.Quantity--;
		return referenceBlock.defaultState;
	}

	//protected override CompoundTooltip GetDefaultTooltip() {
	//	return CompoundTooltip.Of(Tooltip.Info(this.name));
	//}

	public static BlockItem? FromBlock(Block block) => block.itemReference;
}
