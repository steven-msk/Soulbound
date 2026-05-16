using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.Players;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public sealed class PlayerInventoryHandle : IDisposable {
		private readonly UIToolkitItemSlotHandle[] slots;
		private readonly Inventory inventory;
		private readonly ItemRenderManager itemRenderManager;
		private readonly IItemContainerScope scope;
		private VisualElement root;
		private int lastClickedSlot;
		private float lastClickTime;
		const float DOUBLE_CLICK_THRESHOLD = 0.15f;
		const int LEFT_BUTTON = 0;
		const int MIDDLE_BUTTON = 2;
		const int RIGHT_BUTTON = 1;

		public PlayerInventoryHandle(Inventory inventory, ItemRenderManager itemRenderManager, IItemContainerScope scope) {
			this.inventory = inventory;
			this.itemRenderManager = itemRenderManager;
			this.scope = scope;
			this.slots = new UIToolkitItemSlotHandle[inventory.GetSize()];
		}

		public bool isOpen { get; private set; }

		public void OnBind(VisualElement root) {
			this.root = root;

			foreach (var slotIndex in this.inventory.GetPopupSlots()) {
				IItemSlot slot = this.inventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetPopup()[slotIndex - Inventory.HOTBAR_SIZE];

				UIToolkitItemSlotHandle handle = new(slotElement, slot, this.itemRenderManager);
				this.slots[slotIndex] = handle;
				this.AddPointerListeners(slotElement, handle, slot);
			}

			foreach (var slotIndex in this.inventory.GetHotbarSlots()) {
				IItemSlot slot = this.inventory.GetSlot(slotIndex);
				VisualElement slotElement = this.GetHotbar()[slotIndex];

				HotbarSlotHandle handle = new(slotElement, slot, this.itemRenderManager);
				this.slots[slotIndex] = handle;
				this.AddPointerListeners(slotElement, handle, slot);
			}

			this.inventory.mainSlotChanged += this.OnMainSlotChanged;
			this.SetAsMainSlot(this.inventory.GetMainSlot());
		}

		public void Open(Player player) {
			this.GetPopup().style.display = DisplayStyle.Flex;
			this.scope.AddContainer(this.inventory);
			this.inventory.OnOpened(player);
			this.isOpen = true;
		}

		public void Close(Player player) {
			this.GetPopup().style.display = DisplayStyle.None;
			this.scope.RemoveContainer(this.inventory);
			this.inventory.OnClosed(player);
			this.isOpen = false;
		}

		private void OnMainSlotChanged(int oldIndex, int newIndex) {
			this.UnsetMainSlot(oldIndex);
			this.SetAsMainSlot(newIndex);
		}

		private void SetAsMainSlot(int slot) => this.slots[slot].SetAsMainSlot();
		private void UnsetMainSlot(int slot) => this.slots[slot].UnsetMainSlot();

		private void AddPointerListeners(VisualElement visualElement, UIToolkitItemSlotHandle handle, IItemSlot slot) {
			handle.onPointerDown += evt => this.OnPointerDown(slot, visualElement, evt);
			handle.onPointerUp += evt => this.OnPointerUp(slot, visualElement, evt);
			handle.onPointerEnter += evt => this.OnPointerEnter(slot, visualElement, evt);
			handle.onPointerLeave += evt => this.OnPointerLeave(slot, visualElement, evt);
		}

		private void OnPointerDown(IItemSlot slot, VisualElement visualElement, PointerDownEvent evt) {
			float time = Time.time;
			bool doubleClick = this.lastClickedSlot == slot.GetIndex() && (time - this.lastClickTime) <= DOUBLE_CLICK_THRESHOLD;
			this.lastClickTime = time;
			this.lastClickedSlot = slot.GetIndex();

			int clickButton = evt.button;
			ISlotOperation operation = this.GetClick(slot.GetIndex(), clickButton, doubleClick);
			if (operation is NoSlotOperation) return;

			this.scope.TryBeginDrag(
				this.scope.HasTransitStack()
					? this.scope.GetTransitStack()
					: this.inventory.GetSlot(slot.GetIndex()).GetStack(),
				new SlotRef(this.inventory, slot.GetIndex()),
				clickButton
			);
			operation.Execute();
		}
		
		private void OnPointerUp(IItemSlot slot, VisualElement visualElement, PointerUpEvent evt) {
			this.scope.EndDrag();
		}

		private void OnPointerEnter(IItemSlot slot, VisualElement visualElement, PointerEnterEvent evt) {
			if (!this.scope.InDragState()) return;

			int dragButton = this.scope.GetDragState().button;
			ISlotOperation operation = this.GetDrag(slot.GetIndex(), dragButton);
			if (operation is NoSlotOperation) return;

			operation.Execute();
		}

		private void OnPointerLeave(IItemSlot slot, VisualElement visualElement, PointerLeaveEvent evt) {
		}

		private ISlotOperation GetClick(int slotIndex, int clickButton, bool doubleClick) {
			if (clickButton < 0) return new NoSlotOperation();

			if (clickButton == LEFT_BUTTON) {
				CollectAllItemsToTransit collectToTransit = new(this.scope);

				return doubleClick && collectToTransit.CanExecute()
					? collectToTransit
					: new TransferTransit(this.inventory, slotIndex, this.scope);
			}

			if (clickButton == RIGHT_BUTTON) {
				TransferSingleToSlot transferSingleToSlot = new(this.inventory, slotIndex, this.scope);
				HalveStackFromSlot halveStackFromSlot = new(this.inventory, slotIndex, this.scope);

				if (transferSingleToSlot.CanExecute()) return transferSingleToSlot;
				if (halveStackFromSlot.CanExecute()) return halveStackFromSlot;

				return new NoSlotOperation();
			}
			return new NoSlotOperation();
		}

		private ISlotOperation GetDrag(int slotIndex, int button) {
			if (button == LEFT_BUTTON) {
				return new SplitDistributeToDraggedSlot(new SlotRef(this.inventory, slotIndex), this.scope);
			}
			if (button == RIGHT_BUTTON) {
				TransferSingleToSlot transferSingleToSlot = new(this.inventory, slotIndex, this.scope);

				if (transferSingleToSlot.CanExecute()) {
					this.scope.ExtendDrag(new SlotRef(this.inventory, slotIndex));
					return transferSingleToSlot;
				}

				return new NoSlotOperation();
			}
			return new NoSlotOperation();
		}

		private VisualElement GetPopup() => this.root.Q<VisualElement>("Popup");
		private VisualElement GetHotbar() => this.root.Q<VisualElement>("Hotbar");

		public void Dispose() {
			for (int i = 0; i < this.slots.Length; i++) {
				this.slots[i].Dispose();
				this.slots[i] = null;
			}
			this.inventory.mainSlotChanged -= this.OnMainSlotChanged;
		}
	}
}
