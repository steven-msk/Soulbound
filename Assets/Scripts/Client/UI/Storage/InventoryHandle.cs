using SoulboundBackend.Client.Concurrency;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class InventoryHandle : MonoBehaviour, IItemContainerHandle {
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

		void IItemSlotHandleCallbacks.OnPointerDown(int slotIndex, PointerEventData eventData) {
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

			if (!scope.InDragState()) operation.Execute();
			StartDrag(slotIndex, clickButton);
		}

		void IItemSlotHandleCallbacks.OnPointerEnter(int slotIndex, PointerEventData eventData) {
			if (!scope.InDragState()) return;

			ISlotOperation operation = GetDrag(slotIndex, eventData.button);
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
			ItemStack? slotStack = container.GetSlot(slotIndex).GetStack();

			if (clickButton == PointerEventData.InputButton.Left) {
				if (doubleClick && scope.transitStack.HasStack()) {
					return new CollectAllItemsToTransit(scope);
				}
				return new TransferTransit(container, slotIndex, scope);
			} else if (clickButton == PointerEventData.InputButton.Right) {
				if (scope.transitStack.HasStack() && slotStack != null) {
					if (scope.transitStack.GetStack()!.item != slotStack.item) {
						return new NoSlotOperation();
					}
				}
				if (scope.transitStack.HasStack()) {
					return new TransferSingleToSlot(container, slotIndex, scope);
				}
				return new HalveStackFromSlot(container, slotIndex, scope);
			}
			return null!;
		}

		private ISlotOperation GetDrag(int slotIndex, PointerEventData.InputButton button) {
			if (button == PointerEventData.InputButton.Left) {
				return new SplitDistributeToDraggedSlot(slotIndex, container, scope);

			} else if (button == PointerEventData.InputButton.Right) {
				ItemStack? slotStack = container.GetSlot(slotIndex).GetStack();
				ItemStack? transitStack = scope.transitStack.GetStack();

				if (transitStack != null && slotStack != null
						&& transitStack.item != slotStack.item) {
					return new NoSlotOperation();
				}

				scope.ExtendDrag(container, slotIndex);
				return new TransferSingleToSlot(container, slotIndex, scope);
			}
			return null!;
		}

		private void StartDrag(int origin, PointerEventData.InputButton button) {
			IItemSlot slot = container.GetSlot(origin);
			if (!slot.HasStack()) return;

			scope.TryBeginDrag(container, origin, button);
		}

		void IItemSlotHandleCallbacks.OnPointerExit(int slotIndex, PointerEventData eventData) {
		}

		void IItemSlotHandleCallbacks.OnPointerUp(int slotIndex, PointerEventData eventData) => scope.EndDrag();
	}
}
