using SoulboundEngine.Item;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Recipe {
	public abstract record RecipeView(RecipeEntry recipeEntry, ItemStack resultPreview, IReadOnlyList<Ingredient> ingredients);

	public sealed record RecipeView<T>(RecipeEntry<T> entry, ItemStack resultPreview, IReadOnlyList<Ingredient> ingredients)
		: RecipeView(entry, resultPreview, ingredients), IEquatable<RecipeView<T>> where T : IRecipe {
		public override int GetHashCode() {
			throw new NotSupportedException("Do not store RecipeView<T> as a key in a dictionary");
		}

		public bool Equals(RecipeView<T> other) {
			return this.entry.Equals(other.entry);
		}
	}
}
