namespace SoulboundEngine.Client.UI.Screen {
	using SoulboundEngine.Client.Render.Item;
	using SoulboundEngine.Inventory;
	using SoulboundEngine.Item;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;

	public abstract class InventoryScreen<THandler> : UXMLScreen, IInventoryScreenHandlerProvider<THandler> where THandler : InventoryScreenHandler {
		const float DOUBLE_CLICK_THRESHOLD = 0.15f;
		const int LEFT_BUTTON = 0;
		const int MIDDLE_BUTTON = 2;
		const int RIGHT_BUTTON = 1;
		protected readonly THandler handler;
		protected readonly ItemRenderManager itemRenderManager;
		protected readonly PlayerInventory playerInventory;
		protected readonly HashSet<IInventory> openInventories = new();
		protected readonly PlayerEntity player;
		private readonly List<UXMLHotbarSlotDisplay> playerHotbarSlotDisplays = new();
		private TransitStackHandler transitStackHandler;
		private int lastClickedSlot;
		private float lastClickTime;
		private bool dragging;
		private int dragButton;
		private EventModifiers dragModifiers;

		protected InventoryScreen(Context ctx, VisualTreeAsset asset)
			: base(asset) {
			this.handler = ctx.handler;
			this.itemRenderManager = ctx.itemRenderManager;
			this.playerInventory = ctx.playerInventory;
			this.player = ctx.player;
			this.handler.externalTransitStackChange += this.SyncTransitStack;
		}

		protected sealed override void OnBind(VisualElement root) {
			this.transitStackHandler = TransitStackHandler.Create(root, this.itemRenderManager);
			this.OnBindInventory(root);
			root.RegisterCallback<PointerUpEvent>(this.RootOnPointerUp);
		}

		protected virtual void OnBindInventory(VisualElement root) {
			this.BindPlayerInventory(this.playerInventory, this.GetPlayerInventoryRoot(root));
		}

		protected void BindPlayerInventory(PlayerInventory playerInventory, VisualElement inventoryRoot) {
			this.openInventories.Add(playerInventory);

			this.BindPlayerHotbar(playerInventory, inventoryRoot);
			this.BindPlayerPopup(playerInventory, inventoryRoot);

			playerInventory.mainSlotChanged += this.OnMainSlotChanged;
			this.SetMainSlotVisual(playerInventory.GetMainSlot());
		}

		protected abstract VisualElement GetPlayerPopup(VisualElement inventoryRoot);

		protected abstract VisualElement GetPlayerHotbar(VisualElement inventoryRoot);

		protected abstract VisualElement GetPlayerInventoryRoot(VisualElement screenRoot);

		private void OnMainSlotChanged(int oldValue, int newValue) {
			this.UnsetMainSlotVisual(oldValue);
			this.SetMainSlotVisual(newValue);
		}

		protected void BindPlayerPopup(PlayerInventory playerInventory, VisualElement inventoryRoot, bool interactable = true) {
			foreach (int slotIndex in playerInventory.GetPopup()) {
				IItemSlot slot = playerInventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetPlayerPopup(inventoryRoot)[slotIndex - PlayerInventory.HOTBAR_SIZE];

				this.BindSlot(slotElement, slot, playerInventory, interactable);
			}
		}

		protected void BindPlayerHotbar(PlayerInventory playerInventory, VisualElement inventoryRoot, bool interactable = true) {
			foreach (int slotIndex in playerInventory.GetHotbar()) {
				IItemSlot slot = playerInventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetPlayerHotbar(inventoryRoot)[slotIndex];

				UXMLHotbarSlotDisplay display = new(slot, this.itemRenderManager, interactable);
				this.AddWidget(display);
				display.OnBind(slotElement);
				this.AddPointerListeners(slotElement, display, slot, playerInventory);
				this.playerHotbarSlotDisplays.Add(display);
			}
		}

		protected UXMLItemSlotDisplay BindSlot(VisualElement slotElement, IItemSlot slot, IInventory inventory, bool interactable) {
			UXMLItemSlotDisplay display = new(slot, this.itemRenderManager, interactable);
			this.AddWidget(display);
			display.OnBind(slotElement);
			this.AddPointerListeners(slotElement, display, slot, inventory);
			return display;
		}

		private void SetMainSlotVisual(int slot) {
			this.playerHotbarSlotDisplays[slot].SetAsMainSlot();
		}

		private void UnsetMainSlotVisual(int slot) {
			this.playerHotbarSlotDisplays[slot].UnsetMainSlot();
		}

		private void AddPointerListeners(VisualElement visualElement, UXMLItemSlotDisplay display, IItemSlot slot, IInventory inventory) {
			display.onPointerDown += evt => this.OnPointerDown(slot, inventory, visualElement, evt);
			display.onPointerUp += evt => this.OnPointerUp(slot, inventory, visualElement, evt);
			display.onPointerEnter += evt => this.OnPointerEnter(slot, inventory, visualElement, evt);
			display.onPointerLeave += evt => this.OnPointerLeave(slot, inventory, visualElement, evt);
		}

		private void RootOnPointerUp(PointerUpEvent evt) {
			this.EndDrag();
		}

		private void OnPointerUp(IItemSlot slot, IInventory inventory, VisualElement visualElement, PointerUpEvent evt) {
			this.EndDrag();
		}

		protected void EndDrag() {
			if (!this.dragging) return;
			this.dragging = false;
			this.handler.EndDrag();
		}

		private void OnPointerDown(IItemSlot slot, IInventory inventory, VisualElement visualElement, PointerDownEvent evt) {
			float time = Time.time;
			bool doubleClick = this.lastClickedSlot == slot.GetIndex() && (time - this.lastClickTime) <= DOUBLE_CLICK_THRESHOLD;
			this.lastClickTime = time;
			this.lastClickedSlot = slot.GetIndex();

			int clickButton = evt.button;
			EventModifiers modifiers = evt.modifiers;
			int slotIndex = slot.GetIndex();
			try {
				if (this.dragging) this.EndDrag();

				ItemStack originStack = slot.GetStack();
				SlotActionType actionType = this.GetClick(slotIndex, inventory, clickButton, doubleClick, modifiers);
				this.handler.OnSlotAction(slot.GetRef(), clickButton, this.player, actionType);

				ItemStack transitStack = this.handler.GetTransitStack();
				bool stackFromOriginSlot = transitStack.IsEmpty();
				ItemStack dragStack = stackFromOriginSlot ? slot.GetStack() : transitStack;
				if (this.handler.TryStartDrag(dragStack, slot.GetRef(), clickButton, stackFromOriginSlot)) {
					this.dragging = true;
					this.dragButton = clickButton;
					this.dragModifiers = modifiers;
				}

				this.SyncTransitStack(this.handler.GetTransitStack());
			} catch (Exception e) {
				SoulboundEngine.Logger.LogFatal(e);
			}
		}

		public void SyncTransitStack(ItemStack stack) {
			this.transitStackHandler.SetStack(stack);
		}

		private void OnPointerEnter(IItemSlot slot, IInventory inventory, VisualElement visualElement, PointerEnterEvent evt) {
			if (this.dragging) {
				try {
					// Known issue: immediate drag modifiers are unavailable due to PointerEnterEvent.modifiers being event payload
					// The current workaround uses stored dragModifiers when the drag starts
					SlotDragActionType slotDragActionType = this.GetDrag(slot.GetIndex(), inventory, this.dragButton, this.dragModifiers);

					this.handler.OnSlotDrag(slot.GetRef(), this.dragButton, this.player, slotDragActionType);
					this.transitStackHandler.SetStack(this.handler.GetTransitStack());
				} catch (Exception e) {
					SoulboundEngine.Logger.LogFatal(e);
				}
			}
		}

		private void OnPointerLeave(IItemSlot slot, IInventory inventory, VisualElement visualElement, PointerLeaveEvent evt) {
		}

		protected virtual SlotActionType GetClick(int slotIndex, IInventory inventory, int clickButton, bool doubleClick, EventModifiers modifiers) {
			switch (clickButton) {
				case LEFT_BUTTON: {
						return modifiers.HasFlag(EventModifiers.Shift)
							? SlotActionType.QUICK_MOVE
							: this.handler.CanCollectAll() && doubleClick
								? SlotActionType.COLLECT_ALL
								: SlotActionType.PICKUP;
					}
				case RIGHT_BUTTON: {
						return SlotActionType.PICKUP;
					}
				case MIDDLE_BUTTON: {
						return SlotActionType.CLONE;
					}
				default: throw new InvalidOperationException("Unknown slot action button");
			}
		}

		protected virtual SlotDragActionType GetDrag(int slotIndex, IInventory inventory, int clickButton, EventModifiers modifiers) {
			switch (clickButton) {
				case LEFT_BUTTON: {
						return modifiers.HasFlag(EventModifiers.Shift) 
							? SlotDragActionType.QUICK_MOVE 
							: SlotDragActionType.SPLIT;
					}
				case MIDDLE_BUTTON: {
						return SlotDragActionType.CLONE;
					}
				case RIGHT_BUTTON: {
						return SlotDragActionType.INSERT;
					}
				default: throw new InvalidOperationException("Unknown slot action button");
			}
		}

		protected override void OnMouseMoved(MouseMoveEvent evt) {
			base.OnMouseMoved(evt);
			this.transitStackHandler.SetPointerPosition(this.mousePos);
		}

		public THandler GetScreenHandler() => this.handler;

		public override void OnDispose(IScreenHandle handle) {
			base.OnDispose(handle);
			this.playerHotbarSlotDisplays.Clear();
			this.transitStackHandler.Destroy();
			this.playerInventory.mainSlotChanged -= this.OnMainSlotChanged;
			this.handler.externalTransitStackChange -= this.SyncTransitStack;
		}

		public struct Context {
			public THandler handler;
			public PlayerInventory playerInventory;
			public PlayerEntity player;
			public ItemRenderManager itemRenderManager;
		}
	}

}
