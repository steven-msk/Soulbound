using SoulboundEngine.Core.Registry;
using UnityEngine;

namespace SoulboundEngine.Client.Recipe.Asset {
	public abstract class RecipeAsset<TRecipe> : ScriptableObject, IRecipeAsset where TRecipe : IRecipe {
		[SerializeField] private string identifier;
		[SerializeField] private string typeIdentifier;

		public Identifier GetRecipeIdentifier() => Identifier.Of(this.identifier);

		public RecipeType<TRecipe> GetRecipeType() => RecipeType<TRecipe>.From(Identifier.Of(this.typeIdentifier));

		public abstract TRecipe ResolveRecipe();

		public RecipeEntry CreateEntry(RegistryKey<IRecipe> key) {
			return this.CreateEntryTyped(key);
		}

		public RecipeEntry<TRecipe> CreateEntryTyped(RegistryKey<IRecipe> key) {
			return new RecipeEntry<TRecipe>(key, this.ResolveRecipe());
		}

		RecipeType IRecipeAsset.GetRecipeType() => this.GetRecipeType();

		IRecipe IRecipeAsset.ResolveRecipe() => this.ResolveRecipe();
	}
}
