namespace SoulboundEngine.Item.Container {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common;
	using SoulboundEngine.World.Player;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public interface IInventory : IEnumerable<ItemStack>, IClearable {
		IItemSlot GetSlot(int index);

		IEnumerable<int> GetSlots();

		int GetSize();

		bool CanPlayerUse(PlayerEntity player);

		virtual void OnOpened(PlayerEntity player) {
		}

		virtual void OnClosed(PlayerEntity player) {
		}

		IEnumerator<ItemStack> IEnumerable<ItemStack>.GetEnumerator() {
			return this.GetAllSlots().Select(s => s.GetStack()).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		protected static void CreateSimple(IInventory inventory, ref ItemSlot[] slots) {
			slots = new ItemSlot[inventory.GetSize()];

			for (int i = 0; i < inventory.GetSize(); i++) {
				ItemSlot slot = new(inventory, i);
				slots[i] = slot;
			}
		}

		void IClearable.Clear() {
			foreach (int index in this.GetSlots()) {
				this.GetSlot(index).SetStack(ItemStack.EMPTY);
			}
		}
	}

	public static class InventoryDefaults {
		public static JToken Save(this IInventory inventory) {
			return ItemStack.EMPTY_ACCEPTING_CODEC.ListOf().Encode(inventory.ToList());
		}

		public static void Load(this IInventory inventory, JToken json) {
			ItemStack.EMPTY_ACCEPTING_CODEC.ListOf().Decode(json)
				.ResultOrPartial(error => Logger.LogError(error))
				.IfPresent(stacks => {
					inventory.Clear();
					for (int i = 0; i < stacks.Count; i++) {
						inventory.GetSlot(i).SetStack(stacks[i]);
					}
				});
		}

		public static IEnumerable<IItemSlot> GetAllSlots(this IInventory inventory) {
			return inventory.GetSlots().Select(inventory.GetSlot);
		}

		public static IEnumerable<IItemSlot> MapToInstances(this IInventory inventory, IEnumerable<int> slots) {
			return slots.Select(inventory.GetSlot);
		}
	}
}
