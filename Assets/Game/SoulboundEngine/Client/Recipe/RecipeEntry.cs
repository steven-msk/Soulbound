using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.Recipe {
	/// <summary>
	/// Base, untyped wrapper pairing a resolved recipe with its registry key.
	/// </summary>
	public abstract record RecipeEntry(RegistryKey<IRecipe> key, IRecipe recipe) {
		public override string ToString() {
			return this.key.ToString();
		}
	}

	/// <summary>
	/// Strongly-typed recipe entry. Exposes the underlying recipe as <typeparamref name="T"/>
	/// via <c>tRecipe</c> without requiring a downcast at each call site.
	/// </summary>
	public record RecipeEntry<T>(RegistryKey<IRecipe> key, T tRecipe) : RecipeEntry(key, tRecipe) where T : IRecipe;
}
