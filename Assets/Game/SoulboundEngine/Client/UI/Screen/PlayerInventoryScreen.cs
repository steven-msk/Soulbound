using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Recipe;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class PlayerInventoryScreen : InventoryScreen<PlayerInventoryScreenHandler> {
		private static readonly Identifier HOTBAR_ELEMENT = Identifier.Of("soulbound:hotbar/hotbar");
		private static readonly Identifier POPUP_ELEMENT = Identifier.Of("soulbound:player_inventory/popup");
		private static readonly Identifier PLAYER_INVENTORY_SPACE_ELEMENT = Identifier.Of("soulbound:player_inventory_screen/player_inventory_space");
		private static readonly Identifier CRAFTING_ELEMENT = Identifier.Of("soulbound:player_inventory_screen/crafting");
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
			return inventoryRoot.Get<VisualElement>(HOTBAR_ELEMENT);
		}

		protected override VisualElement GetPlayerPopup(VisualElement inventoryRoot) {
			return inventoryRoot.Get<VisualElement>(POPUP_ELEMENT);
		}

		protected override VisualElement GetPlayerInventoryRoot(VisualElement screenRoot) {
			return screenRoot.Get<VisualElement>(PLAYER_INVENTORY_SPACE_ELEMENT);
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
			List<RecipeView<StationlessCraftingRecipe>> recipeList = recipes.ToList();

			for (int i = 0; i < recipeList.Count; i++) {
				RecipePreviewElement preview = this.AddCraftingPreview(recipeList[i], this.craftingRoot, i);
				this.craftingResultPreviews.Add(preview);
			}
		}

		private RecipePreviewElement AddCraftingPreview(RecipeView<StationlessCraftingRecipe> recipe, VisualElement craftingRoot, int index) {
			TemplateContainer visualElement = this.recipeResultAsset.Instantiate();
			craftingRoot.Add(visualElement);

			RecipePreviewElement preview = new(this.itemRenderManager, visualElement, this.OnCraftingPreviewClicked);
			this.AddWidget(preview);
			preview.Bind(recipe.entry, recipe.resultPreview);
			return preview;
		}

		private void OnCraftingPreviewClicked(ClickEvent evt, RecipePreviewElement preview) {
			this.handler.CraftRecipe(preview.recipe);
			this.SyncTransitStack(this.handler.GetTransitStack());
		}

		private VisualElement GetCraftingPreviewParent(VisualElement screenRoot) {
			return screenRoot.Get<VisualElement>(CRAFTING_ELEMENT);
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

	class RecipePreviewElement : UXMLItemSlotDisplay {
		private readonly VisualElement visualElement;
		private readonly EventCallback<ClickEvent, RecipePreviewElement> onClick;

		public RecipePreviewElement(ItemRenderManager itemRenderManager, VisualElement visualElement, EventCallback<ClickEvent, RecipePreviewElement> onClick)
			: base(itemRenderManager, true, true) {
			this.visualElement = visualElement;
			this.onClick = onClick;
		}

		public RecipeEntry<StationlessCraftingRecipe> recipe { get; private set; }
		public ItemStack result { get; private set; }

		public void Bind(RecipeEntry<StationlessCraftingRecipe> recipe, ItemStack result) {
			this.recipe = recipe;
			this.result = result;
			this.visualElement.RegisterCallback(this.onClick, this);
			this.OnBind(this.visualElement, result);
		}

		public override void Dispose() {
			base.Dispose();
			this.visualElement.UnregisterCallback(this.onClick);
			this.visualElement.RemoveFromHierarchy();
		}
	}
}
