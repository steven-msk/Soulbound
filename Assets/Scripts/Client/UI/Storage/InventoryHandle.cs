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
		private bool dragging = false;
		private SlotDragContext dragCtx;
		private IItemContainer container = null!;
		private TransitStack transitStack = null!;

		public void Init(IItemContainer container, TransitStack transitStack) {
			this.container = container;
			this.transitStack = transitStack;
		}

		public void SetVisible(bool visible) {
			throw new NotImplementedException();
		}

		void IItemSlotHandleCallbacks.OnPointerDown(int slotIndex, PointerEventData eventData) {
			float time = Time.time;
			bool doubleClick = lastClickedSlot == slotIndex && (time - lastClickTime) <= doubleClickThreshold;
			lastClickTime = time;
			lastClickedSlot = slotIndex;

			// somewhere along the line should be the following commented section
			// although this logic doesnt belong here

			//PlayerController player = Soulbound.instance.GetPlayerInstance();
			//void SetRightClickAvailable(bool enabled) {
			//	Action<ItemUseTrigger[]> action = enabled ? player.ItemUsageHandler.Enable : player.ItemUsageHandler.Disable;
			//	action.Invoke(new ItemUseTrigger[] { ItemUseTrigger.RightClick, ItemUseTrigger.RightHold });
			//}

			PointerEventData.InputButton clickButton = eventData.button;
			ISlotOperation operation = GetClick(slotIndex, clickButton, doubleClick);
			if (operation == null) {
				UnityEngine.Debug.LogException(new NullReferenceException("GetClick() returned null"));
				return;
			}

			if (!dragging) operation.Execute();
			StartDrag(slotIndex, clickButton);

			// temporarily remove action resolver

			//actionResolver.Submit(Request.New()
			//	.UnderToken(PlayerActionTokens.SlotClick)
			//	.Execute(() => {
			//})
			//.OnCondition(() => clickedSlot.Handshake(GrabbedContext.value, SlotInteractionMode.Click))
			//.WithPriority(PlayerActionTokens.SlotClick.effectivePriority)
			//.Suppress(PlayerActionTokens.ItemUse, () => !leftHold)
			//.Suppress(PlayerActionTokens.Attack, () => !leftHold)
			//);


			//ExecuteOnGrabbedReference(clickedSlot, (slot, grabbedReference) => {
			//	interpretationFunction.Invoke(slot!, grabbedReference);
			//	hotbar.OnItemTransfer(slot!, grabbedReference);
			//	if (GrabbedContext.value != null && !doubleClick && !cancelDrag) {
			//		activeDragHandler = this.StartDrag(slot!, dragButton);
			//	}
			//});
		}

		void IItemSlotHandleCallbacks.OnPointerEnter(int slotIndex, PointerEventData eventData) {
			if (!dragging) return;

			ISlotOperation operation = GetDrag(slotIndex);
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
				if (doubleClick && transitStack.HasStack()) {
					return new CollectAllItemsToTransit(transitStack.GetStack()?.item, container, slotIndex, transitStack);
				}
				return new TransferTransit(container, slotIndex, transitStack);
			} else if (clickButton == PointerEventData.InputButton.Right) {
				if (transitStack.HasStack() && slotStack != null) {
					if (transitStack.GetStack()!.item != slotStack.item) {
						return new NoSlotOperation();
					}
				}
				if (transitStack.HasStack()) {
					return new TransferSingleToSlot(container, slotIndex, transitStack);
				}
				return new HalveStackFromSlot(container, slotIndex, transitStack);
			}
			return null!;
		}

		private ISlotOperation GetDrag(int slotIndex) {
			if (dragCtx.button == PointerEventData.InputButton.Left) {
				return new SplitDistributeToDraggedSlot(slotIndex, container, dragCtx);

			} else if (dragCtx.button == PointerEventData.InputButton.Right) {
				ItemStack? slotStack = container.GetSlot(slotIndex).GetStack();
				ItemStack? transitStack = this.transitStack.GetStack();

				if (transitStack != null && slotStack != null
						&& transitStack.item != slotStack.item) {
					return new NoSlotOperation();
				}
				dragCtx.draggedSlots.Add(slotIndex);
				return new TransferSingleToSlot(container, slotIndex, this.transitStack);
			}
			return null!;
		}

		private void StartDrag(int origin, PointerEventData.InputButton clickButton) {
			IItemSlot slot = container.GetSlot(origin);
			if (!slot.HasStack() || dragging) return;

			dragging = true;
			dragCtx = new SlotDragContext() {
				item = slot.GetStack()!.item,
				origin = origin,
				draggedSlots = new HashSet<int>() { origin },
				button = clickButton,
				quantitySnapshots = container.GetAllSlots_indexed()
					.Where(i => container.GetSlot(i).GetStack()?.quantity > 0)
					.ToDictionary(i => i, i => container.GetSlot(i).GetStack()!.quantity),
				originStack = slot.GetStack()!.quantity
			};
		}

		private void EndDrag() => dragging = false;

		void IItemSlotHandleCallbacks.OnPointerExit(int slotIndex, PointerEventData eventData) {
		}

		void IItemSlotHandleCallbacks.OnPointerUp(int slotIndex, PointerEventData eventData) => EndDrag();
	}
}
