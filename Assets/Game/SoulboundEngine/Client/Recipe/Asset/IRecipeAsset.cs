using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.Recipe.Asset {
	public interface IRecipeAsset {
		RecipeType GetRecipeType();
		Identifier GetRecipeIdentifier();

		IRecipe ResolveRecipe();
		RecipeEntry CreateEntry(RegistryKey<IRecipe> key);
	}
}
