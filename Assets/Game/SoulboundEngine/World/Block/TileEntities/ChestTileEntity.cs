namespace SoulboundEngine.World.Block.Entity {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Inventory;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Player;
	using System.Collections.Generic;
	using System.Linq;

	public class ChestTileEntity : LootableContainerTileEntity {
		public const int SIZE = 9 * 3;
		public const double MIN_USABLE_DISTANCE = 5.0d;
		private readonly ItemSlot[] slots;

		protected ChestTileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState)
			: base(tileEntityType, blockPos, blockState) {
			IInventory.CreateSimple(this, ref this.slots);
		}

		public override bool CanPlayerUse(PlayerEntity player) {
			return Vec2d.Distance(player.GetPosition(), this.blockPos.GetCenter()) <= MIN_USABLE_DISTANCE;
		}

		public ChestTileEntity(BlockPos blockPos, BlockState blockState) 
			: this(TileEntityType.CHEST, blockPos, blockState) {
		}

		public static ChestTileEntity Create(BlockPos blockPos, BlockState blockState) {
			return new ChestTileEntity(blockPos, blockState);
		}

		protected override InventoryScreenHandler CreateScreenHandler(PlayerInventory playerInventory, PlayerEntity player) {
			return new ChestInventoryScreenHandler(InventoryScreenHandlerType.CHEST, playerInventory, this);
		}

		public override int GetSize() => SIZE;

		public override IItemSlot GetSlot(int index) => this.slots[index];

		public override IEnumerable<int> GetSlots() => this.slots.Select(s => s.GetIndex());

	}
}
