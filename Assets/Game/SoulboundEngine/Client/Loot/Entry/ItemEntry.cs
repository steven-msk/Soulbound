using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Loot.Condition;
using SoulboundEngine.Client.Loot.Context;
using SoulboundEngine.Client.Loot.Function;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Loot.Entry {
	using Item = Item.Item;

	public class ItemEntry : LeafEntry {
		private readonly RegistryEntry<Item> item;

		private ItemEntry(RegistryEntry<Item> item, int weight, int quality, List<ILootCondition> conditions, List<ILootFunction> functions) 
			: base(weight, quality, conditions, functions) {
			this.item = item;
		}

		public static BasicBuilder Create(IItemConvertible drop) {
			return new BasicBuilder((weight, quality, conditions, functions) => {
				return new ItemEntry(Items.GetEntry(drop.AsItem()), weight, quality, conditions, functions);
			});
		}

		protected override void GenerateLoot(Action<ItemStack> lootConsumer, LootContext context) {
			ItemStack stack = this.item.GetValue().CreateStack();
			lootConsumer.Invoke(stack);
		}
	}
}
