using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	[Obsolete]
	public class InventoryHandle : MonoBehaviour, IItemContainerHandle {
		private float lastClickTime;
		private int lastClickedSlot;
		const float doubleClickThreshold = 0.15f;
		private IItemContainer container = null!;
		private IItemContainerScope scope = null!;

		public void Init(IItemContainer container, IItemContainerScope scope) {
			this.container = container;
			this.scope = scope;
		}

		public void SetVisible(bool visible) {
			throw new NotImplementedException();
		}

		void IItemSlotEventListener.OnPointerDown(int slotIndex, PointerEventData eventData) {
			float time = Time.time;
			bool doubleClick = this.lastClickedSlot == slotIndex && (time - this.lastClickTime) <= doubleClickThreshold;
			this.lastClickTime = time;
			this.lastClickedSlot = slotIndex;

			PointerEventData.InputButton clickButton = eventData.button;
			ISlotOperation operation = this.GetClick(slotIndex, clickButton, doubleClick);
			if (operation == null) {
				UnityEngine.Debug.LogException(new NullReferenceException("GetClick() returned null"));
				return;
			}

			//scope.TryBeginDrag(
			//	scope.HasTransitStack()
			//		? scope.GetTransitStack()!
			//		: container.GetSlot(slotIndex).GetStack()!,
			//	new SlotRef(container, slotIndex),
			//	clickButton
			//);
			operation.Execute();
		}

		void IItemSlotEventListener.OnPointerEnter(int slotIndex, PointerEventData eventData) {
			if (!this.scope.InDragState()) return;

			// eventData.button does not retain drag click button because its called OnPointerEnter
			//PointerEventData.InputButton dragButton = scope.GetDragState()!.button;
			PointerEventData.InputButton dragButton = default;

			ISlotOperation operation = this.GetDrag(slotIndex, dragButton);
			if (operation == null) {
				UnityEngine.Debug.LogException(new InvalidOperationException("GetDrag() returned null"));
				return;
			}

			operation.Execute();
		}

		private ISlotOperation GetClick(int slotIndex, PointerEventData.InputButton clickButton, bool doubleClick) {
			bool shift = Keyboard.current.shiftKey.isPressed;
			bool ctrl = Keyboard.current.ctrlKey.isPressed;
			bool alt = Keyboard.current.altKey.isPressed;

			if (clickButton == PointerEventData.InputButton.Left) {
				CollectAllItemsToTransit collectToTransit = new(this.scope);

				return doubleClick && collectToTransit.CanExecute()
					? collectToTransit
					: new TransferTransit(this.container, slotIndex, this.scope);
			}

			if (clickButton == PointerEventData.InputButton.Right) {
				TransferSingleToSlot transferSingleToSlot = new(this.container, slotIndex, this.scope);
				HalveStackFromSlot halveStackFromSlot = new(this.container, slotIndex, this.scope);

				if (transferSingleToSlot.CanExecute()) return transferSingleToSlot;
				if (halveStackFromSlot.CanExecute()) return halveStackFromSlot;

				return new NoSlotOperation();
			}
			return null!;
		}

		private ISlotOperation GetDrag(int slotIndex, PointerEventData.InputButton button) {
			if (button == PointerEventData.InputButton.Left) {
				return new SplitDistributeToDraggedSlot(new SlotRef(this.container, slotIndex), this.scope);
			}
			if (button == PointerEventData.InputButton.Right) {
				TransferSingleToSlot transferSingleToSlot = new(this.container, slotIndex, this.scope);

				if (transferSingleToSlot.CanExecute()) {
					this.scope.ExtendDrag(new SlotRef(this.container, slotIndex));
					return transferSingleToSlot;
				}

				return new NoSlotOperation();
			}
			return null!;
		}

		void IItemSlotEventListener.OnPointerExit(int slotIndex, PointerEventData eventData) {
		}

		void IItemSlotEventListener.OnPointerUp(int slotIndex, PointerEventData eventData) => this.scope.EndDrag();
	}
}
