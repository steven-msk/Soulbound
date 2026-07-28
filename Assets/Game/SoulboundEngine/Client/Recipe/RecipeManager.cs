using SoulboundEngine.Client.Recipe.Asset;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.Recipe {
	/// <summary>
	/// Central runtime entry point for recipe data. Owns the resolved recipe registry
	/// (built once at construction from recipe assets) and the ingredient index lookup,
	/// and exposes typed query APIs for retrieving and matching recipes against inputs.
	/// </summary>
	public sealed class RecipeManager : IRecipeIngredientManager {
		private static readonly RegistryKey<Registry<IRecipe>> REGISTRY = RegistryKey<IRecipe>.OfRegistry(Identifier.Of("recipe"));
		private readonly ResolvedRecipes resolvedRecipes;
		private readonly Dictionary<RegistryKey<RecipeIngredientIndex>, RecipeIngredientIndex> ingredientIndices;

		/// <summary>
		/// Builds the recipe manager by eagerly resolving all recipe assets into a
		/// <see cref="ResolvedRecipes"/> snapshot, and evaluating all ingredient
		/// index registry entries into a lookup dictionary.
		/// </summary>
		/// <param name="entryLookup">Provides access to registered <see cref="RecipeIngredientIndex"/> entries.</param>
		/// <param name="recipeAssetResolver">Resolves recipe assets (loaded via <see cref="AssetManager"/>) into recipe entries.</param>
		public RecipeManager(IRegistryEntryLookup<RecipeIngredientIndex> entryLookup, IRecipeAssetResolver recipeAssetResolver) {
			this.ingredientIndices = entryLookup.GetAllKeys().ToDictionary(key => key, key => entryLookup.Get(key)!.GetValue());
			this.resolvedRecipes = recipeAssetResolver.Resolve(REGISTRY);
		}

		/// <summary>
		/// Gets the ingredient index registered under <paramref name="key"/>.
		/// </summary>
		/// <exception cref="KeyNotFoundException">No index is registered under <paramref name="key"/>.</exception>
		public RecipeIngredientIndex GetIngredientIndex(RegistryKey<RecipeIngredientIndex> key) {
			return this.ingredientIndices.TryGetValue(key, out RecipeIngredientIndex index)
				? index
				: throw new KeyNotFoundException("Ingredient index key not found: " + key);
		}

		/// <summary>
		/// Gets a read-only view of all registered ingredient indices, keyed by their registry key.
		/// </summary>
		public IReadOnlyDictionary<RegistryKey<RecipeIngredientIndex>, RecipeIngredientIndex> GetIngredientIndices() {
			return this.ingredientIndices;
		}

		/// <summary>
		/// Looks up the recipe registered under <paramref name="key"/> and returns it only if its
		/// runtime <see cref="RecipeType"/> matches <paramref name="type"/>; otherwise returns null.
		/// </summary>
		public RecipeEntry<T>? Get<T>(RecipeType<T> type, RegistryKey<IRecipe> key) where T : IRecipe {
			RecipeEntry? untyped = this.resolvedRecipes.Get(key);
			return (untyped?.recipe.GetRecipeType().Equals(type) ?? false)
				? untyped as RecipeEntry<T> : null;
		}

		public bool TryGet<T>(RecipeType<T> type, RegistryKey<IRecipe> key, out RecipeEntry<T> entry) where T : IRecipe {
			entry = this.Get(type, key)!;
			return entry != null;
		}

		/// <summary>
		/// Looks up the untyped recipe entry registered under <paramref name="key"/>, or null if none exists.
		/// </summary>
		public RecipeEntry? Get(RegistryKey<IRecipe> key) {
			return this.resolvedRecipes.Get(key);
		}

		public bool TryGet(RegistryKey<IRecipe> key, out RecipeEntry entry) {
			entry = this.Get(key)!;
			return entry != null;
		}

		/// <summary>
		/// Returns the first recipe of <paramref name="type"/> whose <see cref="IRecipe{TInput}.Matches"/>
		/// succeeds against <paramref name="input"/>, or null if none match.
		/// </summary>
		public RecipeEntry<T>? GetFirstMatch<T, I>(RecipeType<T> type, I input) where T : IRecipe<I> where I : IRecipeInput {
			return this.resolvedRecipes.FindMatching(type, input).FirstOrDefault();
		}

		public bool TryGetFirstMatch<T, I>(RecipeType<T> type, I input, out RecipeEntry<T> entry) where T : IRecipe<I> where I : IRecipeInput {
			entry = this.GetFirstMatch(type, input)!;
			return entry != null;
		}

		/// <summary>
		/// Returns every recipe of <paramref name="type"/> whose <see cref="IRecipe{TInput}.Matches"/>
		/// succeeds against <paramref name="input"/>.
		/// </summary>
		public IEnumerable<RecipeEntry<T>> GetMatching<T, I>(RecipeType<T> type, I input) where T : IRecipe<I> where I : IRecipeInput {
			return this.resolvedRecipes.FindMatching(type, input);
		}

		/// <summary>
		/// Re-validates a previously matched recipe against a new input, avoiding a full re-scan
		/// of all recipes of <paramref name="type"/> when the previous match is likely still valid.
		/// Returns null if <paramref name="recipe"/> is null or no longer matches.
		/// </summary>
		public RecipeEntry<T>? UsePreviousMatch<T, I>(RecipeType<T> type, I input, RecipeEntry<T>? recipe) where T : IRecipe<I> where I : IRecipeInput {
			return this.GetFirstMatch(type, input, recipe?.key);
		}

		public bool TryUsePreviousMatch<T, I>(RecipeType<T> type, I input, RecipeEntry<T>? recipe, out RecipeEntry<T> entry) where T : IRecipe<I> where I : IRecipeInput {
			entry = this.UsePreviousMatch(type, input, recipe)!;
			return entry != null;
		}

		/// <summary>
		/// Looks up the recipe registered under <paramref name="key"/> and returns it only if it is of
		/// <paramref name="type"/> and matches <paramref name="input"/>. Returns null if <paramref name="key"/>
		/// is null, not found, of the wrong type, or does not match.
		/// </summary>
		public RecipeEntry<T>? GetFirstMatch<T, I>(RecipeType<T> type, I input, RegistryKey<IRecipe>? key) where T : IRecipe<I> where I : IRecipeInput {
			if (key == null) return null;
			RecipeEntry<T>? entry = this.Get(type, key);
			return (entry?.tRecipe.Matches(input) ?? false) ? entry : null;
		}

		public bool TryGetFirstMatch<T, I>(RecipeType<T> type, I input, RegistryKey<IRecipe>? key, out RecipeEntry<T> entry) where T : IRecipe<I> where I : IRecipeInput {
			entry = this.GetFirstMatch(type, input, key)!;
			return entry != null;
		}

		public IEnumerable<RecipeEntry> AllEntries() => this.resolvedRecipes.AllRecipes();
	}
}
