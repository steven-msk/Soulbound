namespace SoulboundEngine.Inventory {
	using SoulboundEngine.Item;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public abstract class InventoryScreenHandler {
		private readonly List<SlotRef> slots = new();
		protected readonly InventoryScreenHandlerType type;
		protected ItemStack transitStack;
		private SlotDragState? dragState;
		public event Action<ItemStack>? externalTransitStackChange;

		protected InventoryScreenHandler(InventoryScreenHandlerType type) {
			this.type = type;
		}

		protected virtual int InsertSingleButton => 1;
		protected virtual int PickupHalfButton => 1;
		protected virtual int InsertButton => 0;

		protected void AddPlayerInventorySlots(PlayerInventory playerInventory) {
			this.slots.AddRange(GetRefs(playerInventory.GetPopup(), playerInventory));
		}

		protected void AddPlayerHotbarSlots(PlayerInventory playerInventory) {
			this.slots.AddRange(GetRefs(playerInventory.GetHotbar(), playerInventory));
		}

		protected void AddPlayerArmorSlots(PlayerInventory playerInventory) {
			this.slots.AddRange(GetRefs(PlayerInventory.EQUIPMENT_SLOT_MAPPING.Keys, playerInventory));
		}

		protected void AddPlayerSlots(PlayerInventory playerInventory) {
			this.AddPlayerInventorySlots(playerInventory);
			this.AddPlayerHotbarSlots(playerInventory);
			this.AddPlayerArmorSlots(playerInventory);
		}

		protected SlotRef AddSlot(IItemSlot slot) {
			SlotRef slotRef = slot.GetRef();
			this.slots.Add(slotRef);
			return slotRef;
		}

		public static List<SlotRef> GetRefs(IEnumerable<int> slots, IInventory inventory) {
			return slots.Select(s => new SlotRef(inventory, s)).ToList();
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
		protected virtual void InternalSlotAction(SlotRef slotRef, int button, PlayerEntity player, SlotActionType actionType) {
			IItemSlot slot = slotRef.GetSlot();
			ItemStack slotStack = slot.GetStack();
			switch (actionType) {
				case SlotActionType.COLLECT_ALL:
					Item item = this.transitStack.GetItem();
					List<IItemSlot> slots = this.GetSlotsContaining(item);
					if (slots == null || slots.Count == 0) return;
					HashSet<IInventory> contentUpdates = new();

					foreach (IItemSlot s in slots) {
						ItemStack stack = s.GetStack();
						this.transitStack.FillFrom(ref stack);
						contentUpdates.Add(s.GetInventory());
						s.SetStack(stack);
					}

					foreach (IInventory inventory in contentUpdates) {
						this.OnContentChanged(inventory);
					}
					break;

				case SlotActionType.PICKUP:
					bool hasTransitStack = !this.transitStack.IsEmpty();
					bool hasSlotStack = !slotStack.IsEmpty();

					if (hasTransitStack && hasSlotStack) {
						if (button == this.InsertButton && !this.IsValidPickupOrInsert(this.transitStack, slot)) {
							if (this.CanInsertIntoSlot(this.transitStack, slot)) {
								this.SwapTransitWithSlot(slot);
							}
							return;
						}
						if (this.IsValidPickupOrInsert(this.transitStack, slot)) {
							if (button == this.InsertSingleButton && this.CanInsertIntoSlot(slot)) {
								this.InsertSingle(slot);
							} else if (button == this.InsertButton && this.CanInsertIntoSlot(slot)) {
								if (slotStack.IsFull()) {
									this.SwapTransitWithSlot(slot);
								} else {
									this.InsertTransitInSlot(slot);
								}
							}
						}
					} else {
						if (slot.HasStack()) {
							if (button == this.PickupHalfButton) {
								this.PickupHalf(slot);
							} else if (button == this.InsertButton) {
								this.SwapTransitWithSlot(slot);
							}
						} else if (this.CanInsertIntoSlot(slot)) {
							if (button == this.InsertButton) {
								this.InsertTransitInSlot(slot);
							} else if (button == this.InsertSingleButton) {
								this.InsertSingle(slot);
							}
						}
					}
					break;

				case SlotActionType.CLONE:
					if (this.transitStack.IsEmpty() && !slotStack.IsEmpty()) {
						this.transitStack = slotStack.CopyFullStack();
					} else if (this.CanInsertIntoSlot(slot)) {
						slot.SetStack(this.transitStack.CopyFullStack());
						this.OnContentChanged(slot.GetInventory());
					}
					break;

				case SlotActionType.QUICK_MOVE:
					this.HandleQuickMove(player, slot);
					break;
			}
		}

		/// <summary>
		/// Called when a slot's content has changed.
		/// </summary>
		public virtual void OnContentChanged(IInventory inventory) {
		}

		private void HandleQuickMove(PlayerEntity player, IItemSlot slot) {
			this.QuickMove(player, slot);
			this.OnContentChanged(slot.GetInventory());
		}

		/// <summary>
		/// Quick moves the stack in slot to other slots of the inventory screen handler. <br/>
		/// The target slots may belong to another inventory or a section of the same inventory. <br/>
		/// Subclasses should call <seealso cref="InsertItem"/> and set the stack of the used slot 
		/// as the passed stack reference.
		/// </summary>
		protected abstract void QuickMove(PlayerEntity player, IItemSlot slot);

		public bool TryStartDrag(ItemStack originStack, SlotRef originSlot, int button, bool stackFromOriginSlot = false) {
			if (this.IsDragging()) return false;

			HashSet<SlotRef> draggedSlots = new(new SlotRef.EqualityComparer()) { originSlot };

			this.dragState = new SlotDragState {
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
				case SlotDragActionType.SPLIT:
					if (this.CanInsertIntoSlot(this.dragState.stack, slot) && !this.dragState.IsSlotDragged(slotRef)) {
						if (this.IsValidPickupOrInsert(this.dragState.stack, slot)) {
							this.SplitDistributeToDragged(slotRef);
						}
					}
					break;
				case SlotDragActionType.INSERT:
					if (this.CanInsertIntoSlot(slot) && this.IsValidPickupOrInsert(this.dragState.stack, slot)) {
						this.InsertSingle(ref this.transitStack, slot);
						this.dragState.ExtendDrag(slotRef);
					}
					break;
				case SlotDragActionType.CLONE:
					if (this.CanInsertIntoSlot(this.dragState.stack, slot) && !this.dragState.IsSlotDragged(slotRef)) {
						if (this.IsValidPickupOrInsert(this.dragState.stack, slot)) {
							slot.SetStack(this.dragState.stack.CopyFullStack());
							this.dragState.ExtendDrag(slotRef);
							this.OnContentChanged(slot.GetInventory());
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
			foreach (SlotRef slot in this.slots) {
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
			if (!this.CanInsertIntoSlot(this.dragState.stack, draggedSlotRef.GetSlot())) return;

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
				int finalCount = Math.Min(baseCount + amount, this.dragState.stack.GetItem().GetMaxCount());

				if (baseCount <= 0) {
					draggedSlot.SetStack(this.dragState.stack.GetItem().GetDefaultStack(finalCount));
				} else {
					draggedSlot.SetStack(this.dragState.stack.CopyWithCount(finalCount));
				}

				inserted += finalCount - baseCount;
			}

			this.transitStack = this.dragState.stack.CopyWithCount(toSplit - inserted);
			foreach (IInventory? inventory in this.dragState.inventories) {
				this.OnContentChanged(inventory);
			}
		}

		protected void SwapTransitWithSlot(IItemSlot slot) {
			ItemStack temp = this.transitStack;
			this.transitStack = slot.GetStack();
			slot.SetStack(temp);

			this.OnContentChanged(slot.GetInventory());
		}

		protected void CollectAll(Item item) {
			List<IItemSlot> slots = this.GetSlotsContaining(item);
			if (slots == null || slots.Count == 0) return;
			HashSet<IInventory> contentUpdates = new();

			foreach (IItemSlot slot in slots) {
				ItemStack stack = slot.GetStack();
				this.transitStack.FillFrom(ref stack);
				contentUpdates.Add(slot.GetInventory());
				slot.SetStack(stack);
			}

			foreach (IInventory inventory in contentUpdates) {
				this.OnContentChanged(inventory);
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

			this.OnContentChanged(slot.GetInventory());
		}

		/// <summary>
		/// Tries to append <c>itemStack</c> to transit.
		/// Returns the remaining stack, or <c>ItemStack.EMPTY</c> otherwise.
		/// </summary>
		protected ItemStack Pickup(ItemStack itemStack) {
			if (!itemStack.IsOf(this.transitStack.GetItem()) && !this.transitStack.IsEmpty()) {
				return itemStack;
			}
			if (this.transitStack.IsEmpty()) {
				this.transitStack = itemStack;
				itemStack = ItemStack.EMPTY;
			} else {
				this.transitStack.FillFrom(ref itemStack);
			}
			return itemStack;
		}

		protected void InsertSingle(IItemSlot slot) => this.InsertSingle(ref this.transitStack, slot);

		protected void InsertSingle(ref ItemStack stack, IItemSlot slot) {
			if (!slot.HasStack()) {
				ItemStack cloned = stack.CopyWithCount(1);
				stack.Decrement();
				slot.SetStack(cloned);
				this.OnContentChanged(slot.GetInventory());
				return;
			}

			ItemStack slotStack = slot.GetStack();
			int added = slotStack.Increment();
			if (added > 0) {
				stack.Decrement();
				slot.SetStack(slotStack);
			}

			this.OnContentChanged(slot.GetInventory());
		}

		protected void InsertTransitInSlot(IItemSlot slot) {
			this.transitStack = this.InsertInSlot(this.transitStack, slot);
		}

		protected ItemStack InsertInSlot(ItemStack stack, IItemSlot slot) {
			if (!slot.HasStack()) {
				slot.SetStack(this.transitStack);
				this.OnContentChanged(slot.GetInventory());
				return ItemStack.EMPTY;
			}
			int space = slot.GetStack().GetSpaceLeft();
			if (space <= 0) return stack;

			int transfer = Math.Min(space, this.transitStack.count);

			ItemStack slotStack = slot.GetStack();
			slotStack.Increment(transfer);
			slot.SetStack(slotStack);
			stack.Decrement(transfer);

			this.OnContentChanged(slot.GetInventory());
			return stack;
		}

		/// <summary>
		/// Tries to consume stack by inserting to <paramref name="slots"/> 
		/// until the entire stack is used.
		/// </summary>
		/// <returns>Whether the stack was fully consumed</returns>
		protected bool InsertItem(ref ItemStack stack, IItemSlot[] slots, bool reverse) {
			HashSet<IInventory> contentUpdates = new();

			ItemStack copyOfStack = stack;
			IEnumerable<IItemSlot> targetSlots = slots.Where(s => this.CanInsertIntoSlot(copyOfStack, s));
			if (reverse) targetSlots = targetSlots.Reverse();
			bool consumed = InventoryUtils.TryAddStack(targetSlots, ref stack);

			foreach (IInventory inventory in contentUpdates) {
				this.OnContentChanged(inventory);
			}

			return consumed;
		}

		public bool CanInsertIntoSlot(IItemSlot slot) {
			return this.CanInsertIntoSlot(this.transitStack, slot);
		}

		/// <summary>
		/// Returns whether stack can be inserted into the slot.
		/// Subclasses should override this to return false if the slot is used for output.
		/// </summary>
		public virtual bool CanInsertIntoSlot(ItemStack itemStack, IItemSlot slot) {
			return true;
		}

		public bool IsValidPickupOrInsert(ItemStack stack, IItemSlot slot) {
			return stack.IsEmpty() || !slot.HasStack() || (stack.CanBeStackedWith(slot.GetStack()) && slot.GetStack().CanBeStackedWith(stack));
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

		public virtual void OnClosed(PlayerEntity player) {
		}

		public ItemStack GetTransitStack() => this.transitStack;

		public void SetTransitStack(ItemStack stack) {
			this.transitStack = stack;
			externalTransitStackChange?.Invoke(stack);
		}

		public StackReference GetTransitStackReference() {
			return new StackReference(this.SetTransitStack, this.GetTransitStack);
		}

		public InventoryScreenHandlerType GetHandlerType() => this.type;
	}
}
