using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class BlockItem : ConstructableItemDefinition, IPlaceable {
		public virtual Func<Block> blockGetter { get; }
		public Block referenceBlock => blockGetter() ?? throw new InvalidOperationException("Block reference is not yet initialized.");

		public BlockItem(string name, ItemAspect aspect, int maxStackSize, Func<Block> blockGetter,
				Func<Item, TooltipData?> tooltipSupplier, TooltipRenderer.NodeStyleFactory? nodeStyleProvider = null)
			: base(name, aspect, maxStackSize, tooltipSupplier, nodeStyleProvider) {
			this.blockGetter = blockGetter;
		}

		public BlockItem(string name, ItemAspect aspect, int maxStackSize, Func<Block> blockGetter) 
			: this(name, aspect, maxStackSize, blockGetter, (item) => null, null) { }

		public virtual BlockState Place(ItemStack itemStack, BlockPos position) {
			itemStack.Decrement();
			return referenceBlock.Place(itemStack, position);
		}

		public static BlockItem? FromBlock(Block block) => block.itemReference;
	}
}
