using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class BlockItem : ConstructableItemDefinition, IPlaceable {
		public virtual Func<Block> blockGetter { get; }
		public Block referenceBlock => blockGetter() ?? throw new InvalidOperationException("Block reference is not yet initialized.");

		public BlockItem(string name, ItemAspect aspect, int maxStackSize, Func<Block> blockGetter)
			: base(name, aspect, maxStackSize) {
			this.blockGetter = blockGetter;
		}

		public virtual BlockState Place(ItemStack itemStack, BlockPos position) {
			itemStack.Decrement();
			return referenceBlock.Place(itemStack, position);
		}

		public static BlockItem? FromBlock(Block block) => block.itemReference;
	}
}
