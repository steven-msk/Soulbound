using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Recipe {
	public interface IRecipeResolver {
		ResolvedRecipes Resolve(RegistryKey<Registry<IRecipe>> registryKey);
	}
}
