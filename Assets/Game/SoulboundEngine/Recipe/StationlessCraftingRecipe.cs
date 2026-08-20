using SoulboundEngine.Item;
using System.Collections.Generic;

namespace SoulboundEngine.Recipe {
	public class StationlessCraftingRecipe : IRecipe<InventoryRecipeInput> {
		private readonly List<Ingredient> ingredients;
		private readonly ItemStack result;

		public StationlessCraftingRecipe(List<Ingredient> ingredients, ItemStack result) {
			this.ingredients = ingredients;
			this.result = result;
		}

		public ItemStack Craft(InventoryRecipeInput input) {
			return this.result;
		}

		public RecipeType GetRecipeType() => RecipeType.STATIONLESS;

		public bool Matches(InventoryRecipeInput input) {
			foreach (var ingredient in this.ingredients) {
				if (!input.Contains(ingredient)) {
					return false;
				}
			}
			return true;
		}

		public IReadOnlyList<Ingredient> Ingredients => this.ingredients;
	}
}
