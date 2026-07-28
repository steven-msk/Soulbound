using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Loot;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.World.Block.State;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.World.Block.Entity {
	public class ChestTileEntity : LootableContainerTileEntity {
		public const int SIZE = 9 * 3;
		public const float MIN_DISTANCE = 5f;
		private readonly ItemSlot[] slots;

		protected ChestTileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState)
			: base(tileEntityType, blockPos, blockState) {
			IInventory.CreateSimple(this, ref this.slots);
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

		public override void OnOpened(PlayerEntity player) {

			// PROTOTYPICAL
			static long Mix(long a, long b) {
				ulong x = unchecked((ulong)(a ^ b) + 0x9E3779B97F4A7C15UL);
				x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
				x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
				return unchecked((long)(x ^ (x >> 31)));
			}
			long chestSeed = Mix(this.GetLevel().seed, this.blockPos.GetHashCode());
			this.SetLootTable(LootTables.CHEST_TEST, chestSeed);

			base.OnOpened(player);
		}
	}
}
