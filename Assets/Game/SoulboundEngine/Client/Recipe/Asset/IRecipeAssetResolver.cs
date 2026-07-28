using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.Recipe.Asset {
	public interface IRecipeAssetResolver {
		ResolvedRecipes Resolve(RegistryKey<Registry<IRecipe>> registryKey);
	}
}
