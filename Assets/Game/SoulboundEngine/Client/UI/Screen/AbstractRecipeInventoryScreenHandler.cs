using SoulboundEngine.Item;
using SoulboundEngine.Item.Container;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.UI.Screen {
	public abstract class AbstractRecipeInventoryScreenHandler<T, I> : InventoryScreenHandler where T : IRecipe<I> where I : IRecipeInput {
		protected readonly PlayerInventory playerInventory;
		protected readonly InventoryScreenHandlerContext context;
		protected readonly RecipeType<T> recipeType;

		protected AbstractRecipeInventoryScreenHandler(InventoryScreenHandlerType type, PlayerInventory playerInventory, RecipeType<T> recipeType)
			: this(type, playerInventory, InventoryScreenHandlerContext.EMPTY, recipeType) {
		}

		public AbstractRecipeInventoryScreenHandler(InventoryScreenHandlerType type, PlayerInventory playerInventory, InventoryScreenHandlerContext context, RecipeType<T> recipeType)
			: base(type) {
			this.playerInventory = playerInventory;
			this.context = context;
			this.recipeType = recipeType;
			this.UpdateRecipes();
		}

		public IEnumerable<RecipeView<T>> matchingRecipes { get; private set; } = Enumerable.Empty<RecipeView<T>>();
		public event Action<IEnumerable<RecipeView<T>>>? recipesChanged;

		// move to a dedicated RecipeMatcher once recipe input implementations grow
		public void CraftRecipe(RecipeEntry<T> entry) {
			I input = this.GetInput();
			T recipe = entry.tRecipe;

			if (!recipe.Matches(input)) return; // guard: matchingRecipes may be one frame stale by the time the click lands

			ItemStack result = recipe.Craft(input);
			IItemSlot[] inputSlots = this.GetInputSlots();

			foreach (var ingredient in recipe.Ingredients) {
				this.ConsumeIngredient(ingredient, inputSlots);
			}

			ItemStack remainder = this.Pickup(result);
			if (!remainder.IsEmpty()) {
				if (!this.playerInventory.TryAddStack(ref remainder)) {
					this.context.Run((client, blockPos, level) => {
						level.GetPlayer().DropStack(level, remainder);
					});
				}
			}
		}

		public override void OnContentChanged(IInventory inventory) {
			this.UpdateRecipes();
		}

		public void UpdateRecipes() {
			this.context.Run((client, _, _) => {
				I input = this.GetInput();
				IEnumerable<RecipeEntry<T>> recipes = client.RecipeManager.GetMatching(this.recipeType, input);
				IEnumerable<RecipeView<T>> matching = recipes.Select(r => {
					return new RecipeView<T>(r, r.tRecipe.Craft(input), r.tRecipe.Ingredients);
				});

				if (matching.SequenceEqual(this.matchingRecipes)) return;

				this.matchingRecipes = matching;
				recipesChanged?.Invoke(this.matchingRecipes);
			});
		}

		// recipe consumption logic. 
		// could be moved to a dedicated RecipeMatcher once recipes grow
		protected void ConsumeIngredient(Ingredient ingredient, IEnumerable<IItemSlot> slots) {
			int remaining = ingredient.GetCount();
			HashSet<IInventory> contentUpdates = new();

			foreach (var slot in slots) {
				if (remaining <= 0) break;

				ItemStack stack = slot.GetStack();
				if (stack.IsEmpty() || !ingredient.AcceptsItem(Items.GetEntry(stack.GetItem()))) continue;

				int take = Math.Min(remaining, stack.count);
				stack.Decrement(take);
				slot.SetStack(stack);
				remaining -= take;
				contentUpdates.Add(slot.GetInventory());
			}

			foreach (var inventory in contentUpdates) {
				this.OnContentChanged(inventory);
			}
		}

		public abstract I GetInput();

		public abstract IItemSlot[] GetInputSlots();
	}
}
