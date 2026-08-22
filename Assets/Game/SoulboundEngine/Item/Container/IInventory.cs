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

		public IEnumerable<IItemSlot> GetAllSlots() {
			return this.GetSlots().Select(i => this.GetSlot(i));
		}

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
			JArray array = new();
			for (int i = 0; i < inventory.GetSize(); i++) {
				array.Add(ItemStack.ToJson(inventory.GetSlot(i).GetStack()));
			}
			return array;
		}

		public static void Load(this IInventory inventory, JToken json) {
			if (json.Type != JTokenType.Array) {
				Logger.LogError("Inventory json is not array: {}", json);
				return;
			}

			JArray array = (JArray)json;
			foreach (int slotIndex in inventory.GetSlots()) {
				ItemStack stack = ItemStack.FromJson(array[slotIndex]);
				inventory.GetSlot(slotIndex).SetStack(stack);
			}
		}
	}
}
