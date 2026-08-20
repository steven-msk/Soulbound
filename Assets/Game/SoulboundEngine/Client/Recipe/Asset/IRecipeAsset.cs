using SoulboundEngine.Recipe;
using SoulboundEngine.Registry;

namespace SoulboundEngine.Client.Recipe.Asset {
	public interface IRecipeAsset {
		RecipeType GetRecipeType();
		Identifier GetRecipeIdentifier();

		IRecipe ResolveRecipe();
		RecipeEntry CreateEntry(RegistryKey<IRecipe> key);
	}

	public interface IRecipeAsset<TRecipe> : IRecipeAsset where TRecipe : IRecipe {
		new RecipeType<TRecipe> GetRecipeType();

		new TRecipe ResolveRecipe();

		RecipeEntry IRecipeAsset.CreateEntry(RegistryKey<IRecipe> key) {
			return this.CreateEntryTyped(key);
		}

		public RecipeEntry<TRecipe> CreateEntryTyped(RegistryKey<IRecipe> key) {
			return new RecipeEntry<TRecipe>(key, this.ResolveRecipe());
		}

		RecipeType IRecipeAsset.GetRecipeType() => this.GetRecipeType();

		IRecipe IRecipeAsset.ResolveRecipe() => this.ResolveRecipe();
	}
}
