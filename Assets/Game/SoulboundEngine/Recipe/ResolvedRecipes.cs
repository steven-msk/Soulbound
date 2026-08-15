using SoulboundEngine.Registry;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Recipe {
	/// <summary>
	/// Immutable snapshot of all recipes resolved at startup, indexed both by registry key
	/// (for direct lookup) and by <see cref="RecipeType"/> (for type-scoped matching queries).
	/// </summary>
	public sealed class ResolvedRecipes {
		private readonly Dictionary<RegistryKey<IRecipe>, RecipeEntry> byKey;
		private readonly Dictionary<RecipeType, List<RecipeEntry>> byType;

		private ResolvedRecipes(Dictionary<RegistryKey<IRecipe>, RecipeEntry> byKey, Dictionary<RecipeType, List<RecipeEntry>> byType) {
			this.byKey = byKey;
			this.byType = byType;
		}

		public static ResolvedRecipes Of(IEnumerable<RecipeEntry> recipes) {
			Dictionary<RegistryKey<IRecipe>, RecipeEntry> byKey = recipes.ToDictionary(entry => entry.key);
			Dictionary<RecipeType, List<RecipeEntry>> byType = recipes
				.GroupBy(entry => entry.recipe.GetRecipeType())
				.ToDictionary(g => g.Key, g => g.ToList());

			return new ResolvedRecipes(byKey, byType);
		}

		/// <summary>
		/// Returns all recipes of <paramref name="type"/> whose <see cref="IRecipe{TInput}.Matches"/>
		/// succeeds against <paramref name="input"/>. Returns an empty sequence if no recipes of
		/// this type were resolved.
		/// </summary>
		public IEnumerable<RecipeEntry<T>> FindMatching<I, T>(RecipeType<T> type, I input) where I : IRecipeInput where T : IRecipe<I> {
			return this.GetAll(type).Where(e => e.tRecipe.Matches(input));
		}

		/// <summary>
		/// Looks up the recipe entry registered under <paramref name="key"/>, or null if none exists.
		/// </summary>
		public RecipeEntry? Get(RegistryKey<IRecipe> key) {
			return this.byKey.GetValueOrDefault(key);
		}

		/// <summary>
		/// Returns all resolved recipes of <paramref name="type"/>, or an empty sequence if none
		/// were resolved for that type.
		/// </summary>
		public IEnumerable<RecipeEntry<T>> GetAll<T>(RecipeType<T> type) where T : IRecipe {
			return this.byType.TryGetValue(type, out List<RecipeEntry> recipes)
				? recipes.Cast<RecipeEntry<T>>()
				: Enumerable.Empty<RecipeEntry<T>>();
		}

		public IEnumerable<RecipeEntry> AllRecipes() => this.byType.SelectMany(kvp => kvp.Value);
	}
}
