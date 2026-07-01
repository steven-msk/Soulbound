using SoulboundEngine.Client.Item;

namespace SoulboundEngine.Client.Recipe {
	/// <summary>
	/// Marker interface for all recipe types. Implementations have their own
	/// <see cref="RecipeType"/> so untyped code can filter and dispatch by type
	/// without reflection.
	/// </summary>
	public interface IRecipe {
		RecipeType GetRecipeType();
	}

	/// <summary>
	/// A recipe that can be matched against, and crafted from, a specific input.
	/// </summary>
	public interface IRecipe<TInput> : IRecipe where TInput : IRecipeInput {
		/// <summary>
		/// Produces the crafting result for <paramref name="input"/>. Callers should verify
		/// <see cref="Matches"/> first; behavior is undefined for non-matching input.
		/// </summary>
		ItemStack Craft(TInput input);

		/// <summary>
		/// Returns whether this recipe applies to the given <paramref name="input"/>.
		/// </summary>
		bool Matches(TInput input);
	}
}
