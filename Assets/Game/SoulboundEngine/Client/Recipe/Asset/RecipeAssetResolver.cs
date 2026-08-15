using SoulboundEngine.Client.Assets;
using SoulboundEngine.Recipe;
using SoulboundEngine.Registry;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Recipe.Asset {
	public sealed class RecipeAssetResolver : IRecipeResolver {
		private const string ASSET_LABEL = "recipe";
		private readonly IEnumerable<AssetKey> assetKeys;

		public RecipeAssetResolver() {
			this.assetKeys = AssetManager.LoadAllWithLabel(ASSET_LABEL);
		}

		public ResolvedRecipes Resolve(RegistryKey<Registry<IRecipe>> registryKey) {
			List<RecipeEntry> entries = new();

			foreach (var assetKey in this.assetKeys) {
				IRecipeAsset recipeAsset = AssetManager.Resolve<IRecipeAsset>(assetKey);

				RegistryKey<IRecipe> key = RegistryKey<IRecipe>.Of(registryKey, recipeAsset.GetRecipeIdentifier());
				RecipeEntry entry = recipeAsset.CreateEntry(key);

				entries.Add(entry);
			}

			return ResolvedRecipes.Of(entries);
		}
	}
}
