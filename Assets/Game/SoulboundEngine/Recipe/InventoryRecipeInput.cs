using SoulboundEngine.Item;
using SoulboundEngine.Item.Container;
using System.Collections.Generic;

namespace SoulboundEngine.Recipe {
	using Item = Item.Item;

	/// <summary>
	/// Read-only representation of the player's inventory. Do not reuse instances of this class.
	/// </summary>
	public class InventoryRecipeInput : IRecipeInput {
		private readonly IItemSlot[] slots;
		private readonly List<ItemStack> stacks = new();
		private readonly Dictionary<Item, int> itemCounts = new();

		public InventoryRecipeInput(IItemSlot[] slots) {
			this.slots = slots;
			for (int i = 0; i < this.Size(); i++) {
				this.stacks.Add(slots[i].GetStack());
			}
			this.ComputeCounts();
		}

		private void ComputeCounts() {
			this.itemCounts.Clear();
			foreach (var stack in this.GetStacks()) {
				if (stack.IsEmpty()) continue;
				if (this.itemCounts.TryGetValue(stack.GetItem(), out int count)) {
					this.itemCounts[stack.GetItem()] = count + stack.count;
				} else {
					this.itemCounts[stack.GetItem()] = stack.count;
				}
			}
		}

		public ItemStack GetStackInSlot(int slot) {
			return this.stacks[slot];
		}

		public int Size() => this.slots.Length;

		public bool Contains(Ingredient ingredient) {
			foreach (var entry in ingredient.GetMatchingItems()) {
				Item item = entry.GetValue();
				if (this.Count(item) >= ingredient.GetCount()) {
					return true;
				}
			}
			return false;
		}

		public IEnumerable<ItemStack> GetStacks() => this.stacks;

		public int Count(Item item) {
			return this.itemCounts.TryGetValue(item, out int count) ? count : 0;
		}
	}
}
