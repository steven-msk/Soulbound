using SoulboundEngine.Client.Item;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.Recipe {
	using Item = Item.Item;

	public class Ingredient {
		protected readonly IRegistryEntryList<Item> entries;

		protected Ingredient(IRegistryEntryList<Item> entries) {
			this.entries = entries;
		}

		public static Ingredient OfItem(IItemConvertible item) {
			RegistryEntry<Item> entry = Items.GetEntry(item.AsItem());
			IRegistryEntryList<Item>.Direct entryList = new(new List<RegistryEntry<Item>>() { entry });
			return new Ingredient(entryList);
		}

		public static Ingredient OfItems(IEnumerable<IItemConvertible> items) {
			List<RegistryEntry<Item>> entries = items.Select(i => Items.GetEntry(i.AsItem())).ToList();
			IRegistryEntryList<Item>.Direct entryList = new(entries);
			return new Ingredient(entryList);
		}

		public bool AcceptsItem(RegistryEntry<Item> item) => this.entries.Contains(item);

		public bool AcceptsItem(RegistryEntry<Item> item, int count) {
			ItemStack stack = new(item, count);
			return this.AcceptsStack(stack);
		}

		public bool AcceptsStack(ItemStack stack) {
			return this.AcceptsItem(Items.GetEntry(stack.GetItem())) && stack.count >= this.GetCount();
		}

		public virtual int GetCount() => 1;

		public IEnumerable<RegistryEntry<Item>> GetMatchingItems() => this.entries;

		public static bool Matches(Ingredient? ingredient, ItemStack stack) {
			if (ingredient == null) return false;

			return ingredient.AcceptsStack(stack);
		}
	}
}
