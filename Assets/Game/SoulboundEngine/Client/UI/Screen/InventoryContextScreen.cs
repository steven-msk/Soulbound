using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	[Obsolete]
	public class InventoryContextScreen : UxmlScreen, IInputEventHandler, IInventoryScope {
		int IInputEventHandler.priority => 5005;
		private int lastClickedSlot;
		private float lastClickTime;
		const float DOUBLE_CLICK_THRESHOLD = 0.15f;
		const int LEFT_BUTTON = 0;
		const int MIDDLE_BUTTON = 2;
		const int RIGHT_BUTTON = 1;
		const int SLOT_SIZE_PX = 64;
		const int SLOT_MARGIN_PX = 4;
		private readonly VisualTreeAsset slotAsset;
		private IInteractableUIToolkitSlotDisplay[] playerSlotDisplays;
		private Inventory currentExternalInventory;
		private InteractableUIToolkitSlotDisplay[] externalSlotDisplays;
		private readonly ItemRenderManager itemRenderManager;
		private readonly PlayerEntity player;
		private readonly HashSet<Inventory> openInventories = new();
		private TransitStackHandler transitStack;
		private SlotDragState dragState;
		private Vector2 pointerPosition;
		private VisualElement externalInventoryRoot;

		public InventoryContextScreen(ItemRenderManager itemRenderManager, PlayerEntity player) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("InventoryContextScreen"))) {
			this.itemRenderManager = itemRenderManager;
			this.player = player;
			this.slotAsset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("Slot"));
		}

		public override bool IsOpaque => false;

		protected override void OnBind(VisualElement root) {
			this.transitStack = new TransitStackHandler(this.itemRenderManager, root.Q<VisualElement>("TransitStack"));
			this.player.SetTransitStackSource(this.transitStack);

			this.externalInventoryRoot = root.Q<VisualElement>("ExternalInventorySpace");

			this.BindPlayerInventory(this.player.GetInventory(), root.Q<VisualElement>("PlayerInventorySpace"));

			root.RegisterCallback<PointerUpEvent>(this.RootOnPointerUp);
		}

		private void BindPlayerInventory(PlayerInventory playerInventory, VisualElement inventoryRoot) {
			this.playerSlotDisplays = new IInteractableUIToolkitSlotDisplay[playerInventory.GetSize()];
			this.openInventories.Add(playerInventory);

			foreach (var slotIndex in playerInventory.GetPopup()) {
				IItemSlot slot = playerInventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetPopup(inventoryRoot)[slotIndex - PlayerInventory.HOTBAR_SIZE];

				InteractableUIToolkitSlotDisplay display = new(slot, this.itemRenderManager);
				display.OnBind(slotElement);
				this.playerSlotDisplays[slotIndex] = display;
				this.AddPointerListeners(slotElement, display, slot, playerInventory);
			}

			foreach (var slotIndex in playerInventory.GetHotbar()) {
				IItemSlot slot = playerInventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetHotbar(inventoryRoot)[slotIndex];

				InteractableHotbarSlotDisplay handle = new(slot, this.itemRenderManager);
				handle.OnBind(slotElement);
				this.playerSlotDisplays[slotIndex] = handle;
				this.AddPointerListeners(slotElement, handle, slot, playerInventory);
			}

			playerInventory.mainSlotChanged += this.OnMainSlotChanged;
			this.SetMainSlotVisual(playerInventory.GetMainSlot());
		}

		public void SetExternalInventory(Inventory inventory, IInventoryLayout layout, IEnumerable<int> slots) {
			if (this.currentExternalInventory == inventory) return;

			if (this.currentExternalInventory != null) {
				this.openInventories.Remove(this.currentExternalInventory);
				this.DisposeExternalContainer();
			}

			this.externalSlotDisplays = new InteractableUIToolkitSlotDisplay[inventory.GetSize()];
			this.openInventories.Add(inventory);
			this.currentExternalInventory = inventory;

			foreach (var slotIndex in slots) {
				IItemSlot slot = inventory.GetSlot(slotIndex);
				Vector2 coordinates = layout.GetCoordinates(slotIndex);
				VisualElement slotElement = this.CreateExternalSlot(this.externalInventoryRoot, coordinates);

				InteractableUIToolkitSlotDisplay handle = new(slot, this.itemRenderManager);
				handle.OnBind(slotElement);
				this.externalSlotDisplays[slotIndex] = handle;
				this.AddPointerListeners(slotElement, handle, slot, inventory);
			}
		}

		private void OnMainSlotChanged(int oldIndex, int newIndex) {
			this.UnsetMainSlotVisual(oldIndex);
			this.SetMainSlotVisual(newIndex);
		}

		private void SetMainSlotVisual(int slot) {
			this.playerSlotDisplays[slot].SetAsMainSlot();
		}

		private void UnsetMainSlotVisual(int slot) {
			this.playerSlotDisplays[slot].UnsetMainSlot();
		}

		private VisualElement GetPopup(VisualElement playerInventoryRoot) {
			return playerInventoryRoot.Q<VisualElement>("Popup");
		}

		private VisualElement GetHotbar(VisualElement playerInventoryRoot) {
			return playerInventoryRoot.Q<VisualElement>("Hotbar");
		}

		private VisualElement CreateExternalSlot(VisualElement root, Vector2 coordinates) {
			VisualElement element = this.slotAsset.Instantiate();
			element.AddToClassList("slot-offset");
			root.Add(element);

			element.style.position = Position.Absolute;
			float xpos = coordinates.x * (SLOT_SIZE_PX + SLOT_MARGIN_PX) + SLOT_MARGIN_PX;
			float ypos = coordinates.y * (SLOT_SIZE_PX + SLOT_MARGIN_PX) + SLOT_MARGIN_PX;
			element.style.left = xpos;
			element.style.bottom = ypos;

			return element;
		}

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			yield return InputEventListener.ObserveAny(InputTokens.Mouse.position, inputEvent => {
				this.pointerPosition = inputEvent.context.ReadValue<Vector2>();
				Vector2 converted = new(this.pointerPosition.x, UnityEngine.Device.Screen.height - this.pointerPosition.y);
				this.transitStack.SetPointerPosition(converted);
			});
		}

		private void AddPointerListeners(VisualElement visualElement, IInteractableUIToolkitSlotDisplay display, IItemSlot slot, Inventory inventory) {
			display.onPointerDown += evt => this.OnPointerDown(slot, inventory, visualElement, evt);
			display.onPointerUp += evt => this.OnPointerUp(slot, inventory, visualElement, evt);
			display.onPointerEnter += evt => this.OnPointerEnter(slot, inventory, visualElement, evt);
			display.onPointerLeave += evt => this.OnPointerLeave(slot, inventory, visualElement, evt);
		}

		private void RootOnPointerUp(PointerUpEvent evt) => this.EndDrag();

		private void OnPointerDown(IItemSlot slot, Inventory inventory, VisualElement visualElement, PointerDownEvent evt) {
			float time = Time.time;
			bool doubleClick = this.lastClickedSlot == slot.GetIndex() && (time - this.lastClickTime) <= DOUBLE_CLICK_THRESHOLD;
			this.lastClickTime = time;
			this.lastClickedSlot = slot.GetIndex();

			int clickButton = evt.button;
			ISlotOperation operation = this.GetClick(slot.GetIndex(), inventory, clickButton, doubleClick);
			if (operation is NoSlotOperation) return;

			this.TryBeginDrag(
				this.HasTransitStack()
					? this.GetTransitStack()
					: inventory.GetSlot(slot.GetIndex()).GetStack(),
				new SlotRef(inventory, slot.GetIndex()),
				clickButton
			);
			operation.Execute();
		}

		private void OnPointerUp(IItemSlot slot, Inventory inventory, VisualElement visualElement, PointerUpEvent evt) {
			this.EndDrag();
		}

		private void OnPointerEnter(IItemSlot slot, Inventory inventory, VisualElement visualElement, PointerEnterEvent evt) {
			if (!this.InDragState()) return;

			int dragButton = this.GetDragState().button;
			ISlotOperation operation = this.GetDrag(slot.GetIndex(), inventory, dragButton);
			if (operation is NoSlotOperation) return;

			operation.Execute();
		}

		private void OnPointerLeave(IItemSlot slot, Inventory inventory, VisualElement visualElement, PointerLeaveEvent evt) {
		}

		private ISlotOperation GetClick(int slotIndex, Inventory inventory, int clickButton, bool doubleClick) {
			if (clickButton < 0) return new NoSlotOperation();

			if (clickButton == LEFT_BUTTON) {
				CollectAllItemsToTransit collectToTransit = new(this);

				return doubleClick && collectToTransit.CanExecute()
					? collectToTransit
					: new TransferTransit(inventory, slotIndex, this);
			}

			if (clickButton == RIGHT_BUTTON) {
				TransferSingleToSlot transferSingleToSlot = new(inventory, slotIndex, this);
				HalveStackFromSlot halveStackFromSlot = new(inventory, slotIndex, this);

				if (transferSingleToSlot.CanExecute()) return transferSingleToSlot;
				if (halveStackFromSlot.CanExecute()) return halveStackFromSlot;

				return new NoSlotOperation();
			}
			return new NoSlotOperation();
		}

		private ISlotOperation GetDrag(int slotIndex, Inventory inventory, int button) {
			if (button == LEFT_BUTTON) {
				return new SplitDistributeToDraggedSlot(new SlotRef(inventory, slotIndex), this);
			}
			if (button == RIGHT_BUTTON) {
				TransferSingleToSlot transferSingleToSlot = new(inventory, slotIndex, this);

				if (transferSingleToSlot.CanExecute()) {
					this.ExtendDrag(new SlotRef(inventory, slotIndex));
					return transferSingleToSlot;
				}

				return new NoSlotOperation();
			}
			return new NoSlotOperation();
		}

		public bool TryBeginDrag(ItemStack stack, SlotRef slotRef, int button) {
			if (((IInventoryScope)this).InDragState() || stack == null) return false;

			HashSet<SlotRef> draggedSlots = new(new SlotRef.EqualityComparer()) { slotRef };

			this.dragState = new SlotDragState(slotRef.container) {
				stack = stack.Clone(),
				origin = slotRef,
				draggedSlots = draggedSlots,
				button = button,
				quantitySnapshots = this.CreateQuantitySnapshots(),
			};
			return true;
		}

		private Dictionary<SlotRef, int> CreateQuantitySnapshots() {
			Dictionary<SlotRef, int> snapshots = new();

			foreach (var container in this.openInventories) {
				Dictionary<int, int> quantities = this.GetQuantitySnapshotForContainer(container);

				foreach (var kvp in quantities) {
					SlotRef slotRef = new(container, kvp.Key);
					snapshots[slotRef] = kvp.Value;
				}
			}
			return snapshots;
		}

		private Dictionary<int, int> GetQuantitySnapshotForContainer(Inventory inventory) {
			return inventory.GetAllSlots()
					.Where(i => inventory.GetSlot(i).GetStack()?.quantity > 0)
					.ToDictionary(i => i, i => inventory.GetSlot(i).GetStack()!.quantity);
		}

		public ItemStack GetTransitStack() => this.transitStack?.GetStack();

		public bool HasTransitStack() => this.transitStack?.HasStack() ?? false;

		void ITransitStackSource.SetTransitStack(ItemStack itemStack) {
			if (itemStack == null) this.transitStack?.Destroy();
			else this.transitStack?.SetStack(itemStack);
		}

		public SlotDragState GetDragState() => this.dragState;

		public void EndDrag() => this.dragState = null;

		public void ExtendDrag(SlotRef slotRef) {
			this.dragState?.ExtendDrag(slotRef);
		}

		public bool InDragState() => this.dragState != null;

		public IEnumerable<IItemContainer> GetOpenContainers() => this.openInventories;

		public override void OnHide(IScreenHandle handle) {
			SoulboundClient.Instance.InputManager.RemoveHandler(this);
		}

		public override void OnShow(IScreenHandle handle) {
			SoulboundClient.Instance.InputManager.AddHandler(this);
		}

		public override void OnDispose(IScreenHandle handle) {
			for (int i = 0; i < this.playerSlotDisplays.Length; i++) {
				this.playerSlotDisplays[i].Dispose();
			}
			this.DisposeExternalContainer();
			this.player.GetInventory().mainSlotChanged -= this.OnMainSlotChanged;
		}

		private void DisposeExternalContainer() {
			if (this.externalSlotDisplays != null) {
				for (int i = 0; i < this.externalSlotDisplays.Length; i++) {
					this.externalSlotDisplays[i].Dispose();
					this.externalSlotDisplays[i].RemoveFromHierarchy();
				}
				this.externalSlotDisplays = null;
			}
		}
	}
}
