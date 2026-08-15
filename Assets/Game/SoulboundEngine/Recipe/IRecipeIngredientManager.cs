using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

namespace SoulboundEngine.Recipe {
	public interface IRecipeIngredientManager {
		RecipeIngredientIndex GetIngredientIndex(RegistryKey<RecipeIngredientIndex> key);

		IReadOnlyDictionary<RegistryKey<RecipeIngredientIndex>, RecipeIngredientIndex> GetIngredientIndices();
	}
}
