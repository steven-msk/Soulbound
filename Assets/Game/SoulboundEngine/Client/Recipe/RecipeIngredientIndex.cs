using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.Linq;


namespace SoulboundEngine.Client.Recipe {
	using Item = Item.Item;

	public class RecipeIngredientIndex {
		public static readonly RegistryKey<Registry<RecipeIngredientIndex>> REGISTRY = RegistryKey<RecipeIngredientIndex>.OfRegistry(Identifier.Of("recipe_ingredient_index"));
		public static readonly RecipeIngredientIndex EMPTY = Of(Enumerable.Empty<Ingredient>());


		private readonly HashSet<RegistryEntry<Item>> acceptedItems;

		private RecipeIngredientIndex(HashSet<RegistryEntry<Item>> acceptedItems) {
			this.acceptedItems = acceptedItems;
		}

		public static RecipeIngredientIndex Of(IEnumerable<Ingredient> ingredients) {
			return new RecipeIngredientIndex(ingredients.SelectMany(i => i.GetMatchingItems()).ToHashSet());
		}

		private static RegistryKey<RecipeIngredientIndex> Register(string id) {
			Identifier identifier = Identifier.Of(id);
			return RegistryKey<RecipeIngredientIndex>.Of(REGISTRY, identifier);
		}
	}
}

