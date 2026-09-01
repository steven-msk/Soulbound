namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.Inventory;
	using SoulboundEngine.Item;
	using SoulboundEngine.Recipe;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Render.Item;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine.UIElements;

	public class PlayerInventoryScreen : InventoryScreen<PlayerInventoryScreenHandler> {
		private static readonly UXMLBinding<VisualElement> HOTBAR_ELEMENT = new("soulbound:hotbar/hotbar");
		private static readonly UXMLBinding<VisualElement> POPUP_ELEMENT = new("soulbound:player_inventory/popup");
		private static readonly UXMLBinding<VisualElement> PLAYER_INVENTORY_SPACE_ELEMENT = new("soulbound:player_inventory_screen/player_inventory_space");
		private static readonly UXMLBinding<VisualElement> CRAFTING_ELEMENT = new("soulbound:player_inventory_screen/crafting");
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
			return HOTBAR_ELEMENT.Get(inventoryRoot);
		}

		protected override VisualElement GetPlayerPopup(VisualElement inventoryRoot) {
			return POPUP_ELEMENT.Get(inventoryRoot);
		}

		protected override VisualElement GetPlayerInventoryRoot(VisualElement screenRoot) {
			return PLAYER_INVENTORY_SPACE_ELEMENT.Get(screenRoot);
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
			return CRAFTING_ELEMENT.Get(screenRoot);
		}

		public override void OnDispose(IScreenHandle handle) {
			base.OnDispose(handle);
			this.DisposeCraftingPreviews();
			this.handler.recipesChanged -= this.UpdateRecipeDisplays;
		}

		private void DisposeCraftingPreviews() {
			foreach (RecipePreviewElement preview in this.craftingResultPreviews) {
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
