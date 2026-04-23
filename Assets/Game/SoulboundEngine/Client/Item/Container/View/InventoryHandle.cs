using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container.View {
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
			bool doubleClick = lastClickedSlot == slotIndex && (time - lastClickTime) <= doubleClickThreshold;
			lastClickTime = time;
			lastClickedSlot = slotIndex;

			PointerEventData.InputButton clickButton = eventData.button;
			ISlotOperation operation = GetClick(slotIndex, clickButton, doubleClick);
			if (operation == null) {
				UnityEngine.Debug.LogException(new NullReferenceException("GetClick() returned null"));
				return;
			}

			scope.TryBeginDrag(
				scope.HasTransitStack()
					? scope.GetTransitStack()!
					: container.GetSlot(slotIndex).GetStack()!,
				new SlotRef(container, slotIndex),
				clickButton
			);
			operation.Execute();
		}

		void IItemSlotEventListener.OnPointerEnter(int slotIndex, PointerEventData eventData) {
			if (!scope.InDragState()) return;

			// eventData.button does not retain drag click button because its called OnPointerEnter
			PointerEventData.InputButton dragButton = scope.GetDragState()!.button;

			ISlotOperation operation = GetDrag(slotIndex, dragButton);
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
				CollectAllItemsToTransit collectToTransit = new(scope);

				return doubleClick && collectToTransit.CanExecute()
					? collectToTransit
					: new TransferTransit(container, slotIndex, scope);
			}

			if (clickButton == PointerEventData.InputButton.Right) {
				TransferSingleToSlot transferSingleToSlot = new(container, slotIndex, scope);
				HalveStackFromSlot halveStackFromSlot = new(container, slotIndex, scope);

				if (transferSingleToSlot.CanExecute()) return transferSingleToSlot;
				if (halveStackFromSlot.CanExecute()) return halveStackFromSlot;

				return new NoSlotOperation();
			}
			return null!;
		}

		private ISlotOperation GetDrag(int slotIndex, PointerEventData.InputButton button) {
			if (button == PointerEventData.InputButton.Left) {
				return new SplitDistributeToDraggedSlot(new SlotRef(container, slotIndex), scope);
			}
			if (button == PointerEventData.InputButton.Right) {
				TransferSingleToSlot transferSingleToSlot = new(container, slotIndex, scope);

				if (transferSingleToSlot.CanExecute()) {
					scope.ExtendDrag(new SlotRef(container, slotIndex));
					return transferSingleToSlot;
				}

				return new NoSlotOperation();
			}
			return null!;
		}

		void IItemSlotEventListener.OnPointerExit(int slotIndex, PointerEventData eventData) {
		}

		void IItemSlotEventListener.OnPointerUp(int slotIndex, PointerEventData eventData) => scope.EndDrag();
	}
}
