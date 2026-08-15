using SoulboundEngine.Item;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.Recipe {
	using Item = Item.Item;

	public class IngredientStack : Ingredient {
		private readonly int count;

		protected IngredientStack(IRegistryEntryList<Item> entries, int count) 
			: base(entries) {
			this.count = count;
		}

		public static IngredientStack Of(Ingredient ingredient, int count) {
			List<RegistryEntry<Item>> acceptedItems = ingredient.GetMatchingItems().ToList();			
			IRegistryEntryList<Item>.Direct entryList = new(acceptedItems);
			return new IngredientStack(entryList, count);
		}


		public static IngredientStack Of(IItemConvertible item, int count) {
			return Of(OfItem(item), count);
		}

		public override int GetCount() => this.count;
	}
}
