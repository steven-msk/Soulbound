using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.UI.Screen.Slot;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.UI.Screen {
	using Item = Item.Item;

	public abstract class InventoryScreenHandler {
		private readonly List<SlotRef> slots = new();
		private readonly InventoryScreenHandlerType type;
		private ItemStack transitStack;
		private SlotDragState? dragState;

		protected InventoryScreenHandler(InventoryScreenHandlerType type) {
			this.type = type;
		}

		protected virtual int insertSingleButton => 1;
		protected virtual int pickupHalfButton => 1;
		protected virtual int insertButton => 0;

		protected void AddPlayerInventorySlots(PlayerInventory playerInventory) {
			this.slots.AddRange(GetRefs(playerInventory.GetPopup(), playerInventory));
		}

		protected void AddPlayerHotbarSlots(PlayerInventory playerInventory) {
			this.slots.AddRange(GetRefs(playerInventory.GetHotbar(), playerInventory));
		}

		protected void AddPlayerSlots(PlayerInventory playerInventory) {
			this.AddPlayerInventorySlots(playerInventory);
			this.AddPlayerHotbarSlots(playerInventory);
		}

		protected SlotRef AddSlot(IItemSlot slot) {
			SlotRef slotRef = slot.GetRef();
			this.slots.Add(slotRef);
			return slotRef;
		}

		static List<SlotRef> GetRefs(IEnumerable<int> slots, IItemContainer container) {
			return slots.Select(s => new SlotRef(container, s)).ToList();
		}

		/// <summary>
		/// Returns whether the inventory screen handler can be used. <br/>
		/// Subclasses should call this or implement the check itself. <br/>
		/// The implementation should check that the player is near the source position
		/// (like block pos), and that the source (e.g. block) is not destroyed.
		/// </summary>
		public abstract bool CanUse(PlayerEntity player);

		public void OnSlotAction(SlotRef slotRef, int button, PlayerEntity player, SlotActionType actionType) {
			try {
				this.InternalSlotAction(slotRef, button, player, actionType);
			} catch (Exception e) {
				Logger.LogFatal(e);
			}
		}

		// Implementation note:
		// The current implementation is subject to change. 
		// As the game evolves, more QOL features will be available, and this includes inventory management features
		private void InternalSlotAction(SlotRef slotRef, int button, PlayerEntity player, SlotActionType actionType) {
			IItemSlot slot = slotRef.GetSlot();
			ItemStack slotStack = slot.GetStack();
			switch (actionType) {
				case SlotActionType.COLLECT_ALL:
					this.CollectAll(this.transitStack.item);
					break;
				case SlotActionType.PICKUP:
					this.HandlePickup(slot, slotStack, button);
					break;
				case SlotActionType.CLONE:
					this.HandleClone(slot, slotStack);
					break;
				case SlotActionType.QUICK_MOVE:
					this.HandleQuickMove(player, slot);
					break;
			}
		}

		private void HandleQuickMove(PlayerEntity player, IItemSlot slot) {
			this.QuickMove(player, slot);
		}

		/// <summary>
		/// Quick moves the stack in slot to other slots of the inventory screen handler. <br/>
		/// The target slots may belong to another inventory or a section of the same inventory
		/// </summary>
		protected abstract void QuickMove(PlayerEntity player, IItemSlot slot);

		private void HandlePickup(IItemSlot slot, ItemStack slotStack, int button) {
			bool hasTransitStack = !this.transitStack.IsEmpty();
			bool hasSlotStack = !slotStack.IsEmpty();

			if (hasTransitStack && hasSlotStack) {
				this.HandleOccupiedSlotPickup(slot, slotStack, button);
				return;
			}

			this.HandleOpenSlotPickup(slot, slotStack, button);
		}

		private void HandleOccupiedSlotPickup(IItemSlot slot, ItemStack slotStack, int button) {
			if (!this.CanInsertIntoSlot(slot) || slotStack.IsFull() || this.transitStack.IsFull()) {
				this.SwapTransitWithSlot(slot);
				return;
			}

			if (button == this.insertSingleButton) {
				this.InsertSingle(slot);
			} else if (button == this.insertButton) {
				this.InsertInSlot(slot);
			} else {
				this.SwapTransitWithSlot(slot);
			}
		}

		private void HandleOpenSlotPickup(IItemSlot slot, ItemStack slotStack, int button) {
			if (!slotStack.IsEmpty() && slotStack.IsFull()) {
				this.SwapTransitWithSlot(slot);
			} else if (button == this.insertButton && this.CanInsertIntoSlot(slot)) {
				this.InsertInSlot(slot);
			} else if (!slotStack.IsEmpty() && button == this.pickupHalfButton) {
				this.PickupHalf(slot);
			} else if (!this.transitStack.IsEmpty() && button == this.insertSingleButton && this.CanInsertIntoSlot(slot)) {
				this.InsertSingle(slot);
			} else {
				this.SwapTransitWithSlot(slot);
			}
		}

		private void HandleClone(IItemSlot slot, ItemStack slotStack) {
			if (this.transitStack.IsEmpty() && !slotStack.IsEmpty()) {
				this.transitStack = slotStack.CopyFullStack();
			} else if (this.CanInsertIntoSlot(slot)) {
				slot.SetStack(this.transitStack.CopyFullStack());
			}
		}

		public bool TryStartDrag(ItemStack originStack, SlotRef originSlot, int button, bool stackFromOriginSlot = false) {
			if (this.IsDragging()) return false;

			HashSet<SlotRef> draggedSlots = new(new SlotRef.EqualityComparer()) { originSlot };

			this.dragState = new SlotDragState(originSlot.container) {
				stack = originStack.Copy(),
				origin = originSlot,
				draggedSlots = draggedSlots,
				button = button,
				countSnapshot = this.CreateCountSnapshot(),
				stackFromOriginSlot = stackFromOriginSlot,
			};
			return true;
		}

		public void OnSlotDrag(SlotRef slotRef, int button, PlayerEntity player, SlotDragActionType dragActionType) {
			if (!this.IsDragging()) return;
			if (button != this.dragState!.button) return;

			IItemSlot slot = slotRef.GetSlot();
			switch (dragActionType) {
				case SlotDragActionType.SPLIT: {
						if (this.CanInsertIntoSlot(this.dragState.stack, slot) && !this.dragState.IsSlotDragged(slotRef)) {
							this.SplitDistributeToDragged(slotRef);
						}
					}
					break;
				case SlotDragActionType.INSERT: {
						if (this.CanInsertIntoSlot(slot)) {
							this.InsertSingle(ref this.transitStack, slot);
							this.dragState.ExtendDrag(slotRef);
						}
					}
					break;
				case SlotDragActionType.CLONE: {
						if (this.CanInsertIntoSlot(this.dragState.stack, slot) && !this.dragState.IsSlotDragged(slotRef)) {
							slot.SetStack(this.dragState.stack.CopyFullStack());
							this.dragState.ExtendDrag(slotRef);
						}
					}
					break;
				case SlotDragActionType.QUICK_MOVE:
					this.HandleQuickMove(player, slot);
					break;
				default: throw new NotSupportedException("Drag button not supported: " + this.dragState.button);
			}
		}

		public void EndDrag() {
			this.dragState = null;
		}

		public bool IsDragging() => this.dragState != null;

		protected Dictionary<SlotRef, int> CreateCountSnapshot() {
			Dictionary<SlotRef, int> snapshots = new();
			foreach (var slot in this.slots) {
				ItemStack stack = slot.GetSlot().GetStack();
				if (!stack.IsEmpty()) {
					snapshots.Add(slot, stack.count);
				}
			}
			return snapshots;
		}

		protected void SplitDistributeToDragged(SlotRef draggedSlotRef) {
			if (this.dragState == null) return;
			if (!this.dragState.IsEligible(draggedSlotRef)) return;

			int toSplit = this.dragState.stack.count;
			this.dragState.ExtendDrag(draggedSlotRef);

			List<SlotRef> eligibleSlots = this.dragState.draggedSlots
				.Where(this.dragState.IsEligible)
				.ToList();

			int splitAmount = toSplit / eligibleSlots.Count;
			if (splitAmount <= 0) return;

			int remainder = toSplit % eligibleSlots.Count;
			int inserted = 0;

			for (int i = 0; i < eligibleSlots.Count; i++) {
				SlotRef slotRef = eligibleSlots[i];
				IItemSlot draggedSlot = slotRef.GetSlot();
				int amount = splitAmount + (i < remainder ? 1 : 0);
				int baseCount = this.dragState.GetBaseCount(slotRef);
				int finalCount = Math.Min(baseCount + amount, this.dragState.stack.item.fullStackSize);

				if (baseCount <= 0) {
					draggedSlot.SetStack(this.dragState.stack.item.CreateStack(finalCount));
				} else {
					draggedSlot.SetStack(this.dragState.stack.CopyWithCount(finalCount));
				}

				inserted += finalCount - baseCount;
			}

			this.transitStack = this.dragState.stack.CopyWithCount(toSplit - inserted);
		}

		protected void SwapTransitWithSlot(IItemSlot slot) {
			ItemStack temp = this.transitStack;
			this.transitStack = slot.GetStack();
			slot.SetStack(temp);
		}

		protected void CollectAll(Item item) {
			List<IItemSlot> slots = this.GetSlotsContaining(item);
			if (slots == null || slots.Count == 0) return;

			foreach (var slot in slots) {
				ItemStack stack = slot.GetStack();
				this.transitStack.FillFrom(ref stack);
				slot.SetStack(stack);
			}
		}

		protected void PickupHalf(IItemSlot slot) {
			int half = slot.GetStack().count / 2;
			int remainder = slot.GetStack().count % 2;
			int transfer = half + remainder;

			ItemStack halvedTransit = slot.GetStack().CopyWithCount(transfer);
			ItemStack slotStack = slot.GetStack();
			slotStack.Decrement(transfer);
			slot.SetStack(slotStack);
			this.transitStack = halvedTransit;
		}

		protected void InsertSingle(IItemSlot slot) => this.InsertSingle(ref this.transitStack, slot);

		protected void InsertSingle(ref ItemStack stack, IItemSlot slot) {
			if (!slot.HasStack()) {
				ItemStack cloned = stack.CopyWithCount(1);
				stack.Decrement();
				slot.SetStack(cloned);
				return;
			}

			ItemStack slotStack = slot.GetStack();
			int added = slotStack.Increment();
			if (added > 0) {
				stack.Decrement();
				slot.SetStack(slotStack);
			}

		}

		protected void InsertInSlot(IItemSlot slot) {
			if (!slot.HasStack()) {
				slot.SetStack(this.transitStack);
				this.transitStack = ItemStack.EMPTY;
				return;
			}
			int space = slot.GetStack().GetSpaceLeft();
			if (space <= 0) return;

			int transfer = Math.Min(space, this.transitStack.count);

			ItemStack slotStack = slot.GetStack();
			slotStack.Increment(transfer);
			slot.SetStack(slotStack);
			this.transitStack.Decrement(transfer);
		}

		public bool CanInsertIntoSlot(IItemSlot slot) {
			return this.CanInsertIntoSlot(this.transitStack, slot);
		}

		/// <summary>
		/// Returns whether stack can be inserted into the slot.
		/// Subclasses should override this to return false if the slot is used for output.
		/// </summary>
		public virtual bool CanInsertIntoSlot(ItemStack itemStack, IItemSlot slot) {
			if (itemStack.IsEmpty()) return false;
			if (!slot.HasStack()) return true;
			return ItemStack.AreItemsEqual(itemStack, slot.GetStack());
		}

		protected List<IItemSlot> GetSlotsContaining(Item item) {
			return this.slots
				.Select(r => r.GetSlot())
				.Where(s => s.GetStack().IsOf(item))
				.OrderBy(s => s.GetStack().count)
				.ToList();
		}

		public bool CanCollectAll() {
			return !this.transitStack.IsEmpty();
		}

		public ItemStack GetTransitStack() => this.transitStack;

		public InventoryScreenHandlerType GetHandlerType() => this.type;
	}
}
