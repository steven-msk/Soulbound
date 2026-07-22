using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Recipe;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class PlayerInventoryScreen : InventoryScreen<PlayerInventoryScreenHandler> {
		private const string HOTBAR_ELEMENT = "Hotbar";
		private const string POPUP_ELEMENT = "Popup";
		private const string PLAYER_INVENTORY_SPACE_ELEMENT = "PlayerInventorySpace";
		private const string CRAFTING_ELEMENT = "Crafting";
		private IEnumerable<RecipeView<StationlessCraftingRecipe>> currentRecipes;
		private readonly List<RecipePreviewElement> craftingResultPreviews = new();
		private VisualElement craftingRoot;
		private bool isCraftingBound;
		private readonly VisualTreeAsset recipeResultAsset;

		public PlayerInventoryScreen(Context ctx, VisualTreeAsset asset) 
			: base(ctx, asset) {
			this.recipeResultAsset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("StationaryCraftingResultPreview"));
			this.handler.recipesChanged += this.UpdateRecipeDisplays;
			this.UpdateRecipeDisplays(this.handler.matchingRecipes);
		}

		protected override void OnBindInventory(VisualElement root) {
			base.OnBindInventory(root);
			this.BindCrafting(root);
		}

		protected override VisualElement GetPlayerHotbar(VisualElement inventoryRoot) {
			return inventoryRoot.Q<VisualElement>(HOTBAR_ELEMENT);
		}

		protected override VisualElement GetPlayerPopup(VisualElement inventoryRoot) {
			return inventoryRoot.Q<VisualElement>(POPUP_ELEMENT);
		}

		protected override VisualElement GetPlayerInventoryRoot(VisualElement screenRoot) {
			return screenRoot.Q<VisualElement>(PLAYER_INVENTORY_SPACE_ELEMENT);
		}

		private void BindCrafting(VisualElement screenRoot) {
			this.craftingRoot = this.GetCraftingPreviewParent(screenRoot);
			this.isCraftingBound = true;
			this.UpdateRecipeDisplays(this.currentRecipes);
		}

		private void UpdateRecipeDisplays(IEnumerable<RecipeView<StationlessCraftingRecipe>> recipes) {
			this.currentRecipes = recipes;
			this.DisposeCraftingPreviews();
			this.AddCraftingPreviews(recipes);
		}

		private void AddCraftingPreviews(IEnumerable<RecipeView<StationlessCraftingRecipe>> recipes) {
			if (!this.isCraftingBound) return;

			foreach (var recipe in recipes) {
				RecipePreviewElement preview = this.AddCraftingPreview(recipe, this.craftingRoot);
				this.craftingResultPreviews.Add(preview);
			}
		}

		private RecipePreviewElement AddCraftingPreview(RecipeView<StationlessCraftingRecipe> recipe, VisualElement craftingRoot) {
			TemplateContainer visualElement = this.recipeResultAsset.Instantiate();
			craftingRoot.Add(visualElement);

			RecipePreviewElement preview = new(this.itemRenderManager, visualElement, this.OnCraftingPreviewClicked);
			preview.Bind(recipe.entry, recipe.resultPreview);
			return preview;
		}

		private void OnCraftingPreviewClicked(ClickEvent evt, RecipePreviewElement preview) {
			this.handler.CraftRecipe(preview.recipe);
			this.SyncTransitStack();
		}

		private VisualElement GetCraftingPreviewParent(VisualElement screenRoot) {
			return screenRoot.Q<VisualElement>(CRAFTING_ELEMENT);
		}

		public override void OnDispose(IScreenHandle handle) {
			base.OnDispose(handle);
			this.DisposeCraftingPreviews();
			this.handler.recipesChanged -= this.UpdateRecipeDisplays;
		}

		private void DisposeCraftingPreviews() {
			foreach (var preview in this.craftingResultPreviews) {
				preview.Dispose();
			}
			this.craftingResultPreviews.Clear();
		}

	}

	class RecipePreviewElement {
		private readonly ItemRenderManager itemRenderManager;
		private readonly VisualElement visualElement;
		private readonly EventCallback<ClickEvent, RecipePreviewElement> onClick;
		private readonly ItemRenderHandle renderHandle;
		private readonly ItemRenderContext.UIToolkit renderContext;

		public RecipePreviewElement(ItemRenderManager itemRenderManager, VisualElement visualElement, EventCallback<ClickEvent, RecipePreviewElement> onClick) {
			this.itemRenderManager = itemRenderManager;
			this.visualElement = visualElement;
			this.onClick = onClick;
			this.renderHandle = new ItemRenderHandle(this);
			this.renderContext = new ItemRenderContext.UIToolkit { root = visualElement };
		}

		public RecipeEntry<StationlessCraftingRecipe> recipe { get; private set; }
		public ItemStack result { get; private set; }

		public void Bind(RecipeEntry<StationlessCraftingRecipe> recipe, ItemStack result) {
			this.recipe = recipe;
			this.result = result;
			this.visualElement.RegisterCallback(this.onClick, this);
			this.itemRenderManager.Render(this.renderHandle, result, this.renderContext);
		}

		public void Dispose() {
			this.itemRenderManager.Destroy(this.renderHandle, this.renderContext);
			this.visualElement.UnregisterCallback(this.onClick);
			this.visualElement.RemoveFromHierarchy();
		}
	}
}
