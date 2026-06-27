using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.UI.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulboundEngine.Client.World.Block.TileEntity {
	using Level = Level.Level;

	public class ChestTileEntity : TileEntity, IInventoryScreenHandlerFactory, IInventory {
		public const int SIZE = 9 * 3;
		public const float MIN_DISTANCE = 5f;
		private readonly ItemSlot[] slots;

		public ChestTileEntity(TileEntityType<ChestTileEntity> tileEntityType, Level level, BlockPos blockPos) 
			: base(tileEntityType, level, blockPos) {
			IInventory.CreateSimple(this, ref this.slots);
		}

		public bool CanPlayerUse(PlayerEntity player) {
			return Vector2.Distance(player.GetPosition(), (Vector2)this.blockPos) <= MIN_DISTANCE;
		}

		public InventoryScreenHandler Create(PlayerInventory playerInventory, PlayerEntity player) {
			throw new NotImplementedException();
		}

		public int GetSize() => SIZE;

		public IItemSlot GetSlot(int index) => this.slots[index];

		public IEnumerable<int> GetSlots() => this.slots.Select(s => s.GetIndex());
	}
}
