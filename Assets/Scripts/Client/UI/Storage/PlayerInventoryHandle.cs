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
	delegate void SlotFunction(int slotIndex);
	struct DragContext {
		public Item item;
		public int origin;
		public HashSet<int> draggedSlots;
		public PointerEventData.InputButton button;
		public Dictionary<int, int> quantitySnapshots;
		public int originStack;
	}

	public sealed class PlayerInventoryHandle : MonoBehaviour, IItemContainerHandle {
		private float lastClickTime;
		private int lastClickedSlot;
		const float doubleClickThreshold = 0.15f;
		private bool dragging = false;
		private DragContext dragCtx;
		private IItemContainer container = null!;

		public void Init(IItemContainer container) {
			this.container = container;

			// prototypical
			container.GetSlot(0).SetStack(new(Items.woodBlock, 10));
			container.GetSlot(1).SetStack(new(Items.leavesBlock, 100));
		}

		public void SetVisible(bool visible) {
			throw new NotImplementedException();
		}

		void IItemSlotEventCallbacks.OnPointerDown(int slotIndex, PointerEventData eventData) {
			float time = Time.time;
			bool doubleClick = lastClickedSlot == slotIndex && (time - lastClickTime) <= doubleClickThreshold;
			lastClickTime = time;
			lastClickedSlot = slotIndex;

			PointerEventData.InputButton clickButton = eventData.button;
			SlotFunction slotFunction = GetClick(slotIndex, clickButton, doubleClick);
			if (slotFunction == null) {
				UnityEngine.Debug.LogException(new NullReferenceException("GetClick() returned null"));
				return;
			}

			if (!dragging) slotFunction(slotIndex);
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

		private void StartDrag(int origin, PointerEventData.InputButton clickButton) {
			IItemSlot slot = container.GetSlot(origin);
			if (!slot.HasStack() || dragging) return;

			dragging = true;
			dragCtx = new DragContext() {
				item = slot.GetStack()!.item,
				origin = origin,
				draggedSlots = new HashSet<int>() { origin },
				button = clickButton,
				quantitySnapshots = new Dictionary<int, int>(),
				originStack = slot.GetStack()!.quantity
			};
		}

		private void EndDrag() => dragging = false;

		void IItemSlotEventCallbacks.OnPointerEnter(int slotIndex, PointerEventData eventData) {
			if (!dragging) return;

			SlotFunction slotFuction = GetDrag(slotIndex);
			if (slotFuction == null) {
				UnityEngine.Debug.LogException(new InvalidOperationException("GetDrag() returned null"));
				return;
			}

			slotFuction(slotIndex);
		}

		void IItemSlotEventCallbacks.OnPointerExit(int slotIndex, PointerEventData eventData) {
		}

		void IItemSlotEventCallbacks.OnPointerUp(int slotIndex, PointerEventData eventData) => EndDrag();

		private SlotFunction GetClick(int slotIndex, PointerEventData.InputButton clickButton, bool doubleClick) {
			bool shift = Keyboard.current.shiftKey.isPressed;
			bool ctrl = Keyboard.current.ctrlKey.isPressed;
			bool alt = Keyboard.current.altKey.isPressed;
			ItemStack? slotStack = container.GetSlot(slotIndex).GetStack();

			if (clickButton == PointerEventData.InputButton.Left) {
				if (doubleClick && TransitStack.HasStack()) {
					//return _ => CollectAllToTransit(TransitStack.GetStack()!.item);
					return _ => CollectAllToTransit(TransitStack.GetStack()!.item);
				}
				return TransferTransit;
			} else if (clickButton == PointerEventData.InputButton.Right) {
				if (TransitStack.HasStack() && slotStack != null) {
					if (TransitStack.GetStack()!.item != slotStack.item) {
						return DoNothing;
					}
				}
				if (TransitStack.HasStack()) {
					return TransferSingleToSlot;
				}
				return HalveStackFromSlot;
			}
			return null!;
		}

		private SlotFunction GetDrag(int slotIndex) {
			if (dragCtx.button == PointerEventData.InputButton.Left) {
				return SplitDistributeToDraggedSlot;
			} else if (dragCtx.button == PointerEventData.InputButton.Right) {
				ItemStack? slotStack = container.GetSlot(slotIndex).GetStack();
				ItemStack? transitStack = TransitStack.GetStack();

				if (transitStack != null && slotStack != null
						&& transitStack.item != slotStack.item) {
					return DoNothing;
				}
				dragCtx.draggedSlots.Add(slotIndex);
				return TransferSingleToSlot;
			}
			return null!;
		}

		private void SplitDistributeToDraggedSlot(int slotIndex) {
			IItemSlot slot = container.GetSlot(slotIndex);
			IItemSlot originSlot = container.GetSlot(dragCtx.origin);

			if (dragCtx.draggedSlots.Contains(slotIndex)
				|| (slot.HasStack() && slot.GetStack()!.item != originSlot.GetStack()!.item)
				|| (slot.HasStack() && slot.GetStack()!.IsFull())) return;

			// Clone to preview distribution
			HashSet<int> preview = Enumerable.ToHashSet(new List<int>(dragCtx.draggedSlots) { slotIndex });

			int toSplit = dragCtx.originStack;
			int splitAmount = toSplit / (preview.Count);
			if (splitAmount <= 0) return;

			// Commit the slot to dragged list
			dragCtx.draggedSlots.Add(slotIndex);
			int remainder = toSplit % dragCtx.draggedSlots.Count();

			HashSet<int>.Enumerator enumerator = dragCtx.draggedSlots.GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext()) {
				IItemSlot draggedSlot = container.GetSlot(enumerator.Current);
				int amount = splitAmount + (i < remainder ? 1 : 0);

				if (!draggedSlot.HasStack()) draggedSlot.SetStack(new ItemStack(dragCtx.item, amount));

				if (dragCtx.quantitySnapshots.TryGetValue(enumerator.Current, out var snapshot)
						&& enumerator.Current != dragCtx.origin) {
					draggedSlot.GetStack()!.SetQuantity(snapshot + amount);
				} else {
					draggedSlot.GetStack()!.SetQuantity(amount);
				}
				i++;
			}
		}

		private void DoNothing(int slotIndex) { }

		private void TransferTransit(int slotIndex) {
			//PlayerController player = Soulbound.instance.GetPlayerInstance();
			//void SetRightClickAvailable(bool enabled) {
			//	Action<ItemUseTrigger[]> action = enabled ? player.ItemUsageHandler.Enable : player.ItemUsageHandler.Disable;
			//	action.Invoke(new ItemUseTrigger[] { ItemUseTrigger.RightClick, ItemUseTrigger.RightHold });
			//}
			ItemStack? slotStack = container.GetSlot(slotIndex).GetStack();
			ItemStack? transitStack = TransitStack.GetStack();
			if (slotStack == null && transitStack == null) return;

			// Grab if slot has item, grabbed is empty
			if (!TransitStack.HasStack() && slotStack != null) {
				//SetRightClickAvailable(false);
				GrabItemFromSlot(slotIndex);
				return;
			}

			// Release if slot is empty, grabbed has item
			if (TransitStack.HasStack() && slotStack == null) {
				//SetRightClickAvailable(true);
				ReleaseItemInSlot(slotIndex);
				return;
			}


			// Swap if different items or max quantity exists in either stack
			if (slotStack.item != transitStack.item || slotStack.IsFull() || transitStack.IsFull()) {
				SwapTransit(slotIndex);
			} else {
				// Merge in slot for compatible stacks
				MergeTransitInSlot(slotIndex);
				//SetRightClickAvailable(MergeInSlot(slot, grabbedItem));
			}
		}

		private void GrabItemFromSlot(int slotIndex) {
			IItemSlot slot = container.GetSlot(slotIndex);
			if (!slot.HasStack() || TransitStack.HasStack()) return;

			TransitStack.instance.SetStack(slot.GetStack()!);
			slot.SetStack(null);
		}

		private void ReleaseItemInSlot(int slotIndex) {
			IItemSlot slot = container.GetSlot(slotIndex);
			if (slot.HasStack() || !TransitStack.HasStack()) return;

			slot.SetStack(TransitStack.GetStack());
			TransitStack.instance.Release();
		}

		private void SwapTransit(int slotIndex) {
			IItemSlot slot = container.GetSlot(slotIndex);
			if (!slot.HasStack() || !TransitStack.HasStack()) return;

			ItemStack previous = TransitStack.GetStack()!;
			TransitStack.instance.SetStack(slot.GetStack()!);
			slot.SetStack(previous);
		}

		private void MergeTransitInSlot(int slotIndex) {
			IItemSlot slot = container.GetSlot(slotIndex);
			ItemStack? transitStack = TransitStack.GetStack();
			if (transitStack == null) return;

			if (!slot.HasStack()) {
				ReleaseItemInSlot(slotIndex);
				return;
			}
			if (transitStack.item != transitStack.item) return;

			int space = transitStack.item.maxStackSize - slot.GetStack()!.quantity;
			if (space < 0) return;

			int transfer = Math.Min(space, transitStack.quantity);
			slot.GetStack()!.Increment(transfer);
			transitStack.Decrement(transfer);
		}

		private void CollectAllToTransit(Item item) {
			if (TransitStack.GetStack()?.IsFull() ?? true || item == null) return;

			var slots = GetSlotsContaining(item)
				.OrderBy(slot => slot.GetStack()!.quantity)
				.ToList();
			if (slots == null || slots.Count == 0) return;

			ItemStack transitStack = TransitStack.GetStack()!;
			int spaceLeft = item.maxStackSize - transitStack.quantity;
			foreach (var slot in slots) {
				if (spaceLeft <= 0) break;

				int transfer = transitStack.Increment(slot.GetStack()!.quantity);
				slot.GetStack()!.Decrement(transfer);
				spaceLeft -= transfer;
			}
		}

		public void TransferSingleToSlot(int slotIndex) {
			IItemSlot slot = container.GetSlot(slotIndex);
			if (!TransitStack.HasStack()) return;
			ItemStack transitStack = TransitStack.GetStack()!;

			if (!slot.HasStack()) {
				slot.SetStack(new ItemStack(transitStack.item, 1));
				transitStack.Decrement();
			} else if (slot.GetStack()!.Increment() > 0) {
				transitStack.Decrement();
			}
		}

		public void HalveStackFromSlot(int slotIndex) {
			IItemSlot slot = container.GetSlot(slotIndex);
			if (!slot.HasStack() || TransitStack.HasStack()) return;

			int half = slot.GetStack()!.quantity / 2;
			int remainder = slot.GetStack()!.quantity % 2;
			int transfer = half + remainder;
			slot.GetStack()!.Decrement(transfer);
			TransitStack.instance.SetStack(new ItemStack(slot.GetStack()!.item, transfer));
		}

		private IEnumerable<IItemSlot> GetSlotsContaining(Item item) {
			foreach (var slot in container.GetAllSlots()) {
				if (slot.GetStack()?.item == item) {
					yield return slot;
				}	
			}
		}
	}
}
