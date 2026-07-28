using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Recipe {
	public abstract class RecipeType {
		protected static readonly Dictionary<Identifier, RecipeType> typeById = new();

		public static RecipeType<StationlessCraftingRecipe> STATIONLESS = new("stationless");

		public string id { get; }

		protected RecipeType(string id) {
			this.id = id;
			typeById.Add(Identifier.Of(id), this);
			Registry<RecipeType>.Register(Registries.RECIPE_TYPE, id, this);
		}

		public static void Init() {
		}
	}

	public class RecipeType<TRecipe> : RecipeType where TRecipe : IRecipe {
		public RecipeType(string id)
			: base(id) {
		}

		public static RecipeType<TRecipe> From(Identifier id) {
			return (RecipeType<TRecipe>)typeById[id];
		}
	}
}
