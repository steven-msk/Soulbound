using SoulboundEngine.Item;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundEngine.Client.Recipe.Asset {
	using Item = Item.Item;

	[CreateAssetMenu(fileName = "StationlessCraftingRecipe", menuName = "Recipe/Stationless Crafting Recipe")]
	public class StationlessCraftingRecipeAsset : ScriptableObject, IRecipeAsset<StationlessCraftingRecipe> {
		[SerializeField] private string identifier;
		[SerializeField] private string typeIdentifier;
		[SerializeField] private List<SerializedStack> input;
		[SerializeField] private SerializedStack result;

		public Identifier GetRecipeIdentifier() {
			return Identifier.Of(this.identifier);
		}

		public RecipeType<StationlessCraftingRecipe> GetRecipeType() {
			return RecipeType<StationlessCraftingRecipe>.From(Identifier.Of(this.typeIdentifier));
		}

		public StationlessCraftingRecipe ResolveRecipe() {
			List<Ingredient> ingredients = new();
			foreach (var stack in this.input) {
				Item item = Items.Get(Identifier.Of(stack.itemId));
				Ingredient ingredient = IngredientStack.Of(Ingredient.OfItem(item), stack.count);
				ingredients.Add(ingredient);
			}
			Item resultItem = Items.Get(Identifier.Of(this.result.itemId));
			return new StationlessCraftingRecipe(ingredients, new ItemStack(resultItem, this.result.count));
		}
	}
}
