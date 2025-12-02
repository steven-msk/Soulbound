using SoulboundBackend.Client.Concurrency;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Common;
using SoulboundBackend.Common.UI.Storage;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;
using static PlayerInputActions;
using static UnityEngine.InputSystem.DefaultInputActions;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class InventoryController : MonoBehaviour, IItemContainer2D, ISerializable<SerializedInventory>, IStatContextProvider {
		public delegate void InterpretationFunction(IItemSlot slot, RefBox<ItemDisplay> grabbedItem);
		public delegate InterpretationFunction? InterpretationProvider(DragHandler handler, IItemSlot draggedSlot, RefBox<ItemDisplay> grabbedItem);

		private static readonly Logger logger = Logger.CreateInstance();
		public HotbarController Hotbar => hotbar;
		[SerializeField] private HotbarController hotbar;

		[SerializeField] private GameObject popup;
		[SerializeField] private GameObject armorSlots;
		private static Dictionary<int, ArmorSlot> armorSlotsByIndex = new();
		[SerializeField] private bool opened = false;
		public bool IsOpened => opened;

		public GrabbedItemContext GrabbedContext { get; set; } = new(null, null);
		private IItemSlot? lastKnownGrabbedSlot;

		public InventorySlot[] MainPlayerSlots { get; private set; }

		public int Rows => rows;
		[SerializeField] private int rows;

		public int Columns => columns;
		[SerializeField] private int columns;

		private static Dictionary<int, InventorySlot> popupSlotsByIndex = new();
		public InventorySlot[,] popupSlots;
		public InventorySlot this[int row, int col] => popupSlots[row, col];
		IItemSlot IItemContainer2D.this[int row, int column] => this[row, column];
		public IReadOnlyList<IItemSlot> slots => popupSlots.Flatten();

		private PlayerController player = null!;
		private ConcurrentActionResolver actionResolver = null!;
		IStatModificationHost IStatContextProvider.statModificationHost => player.Stats;

		private float doubleClickThreshold = 0.15f;
		private float lastClickTime;
		private IItemSlot? lastClickedSlot;
		private DragHandler? activeDragHandler;
		private IItemSlot? hoveredSlot;
		private bool leftHold;

		[Inject]
		public void Construct(DiContainer container) {
			this.player = container.Resolve<PlayerController>();
			this.actionResolver = container.Resolve<ConcurrentActionResolver>();
			var inputHandler = container.Resolve<InputHandler>();

			hotbar.Construct(this);
			this.SetupGrid();
			this.PreInitState();

			List<InventorySlot> mainPlayerSlots = hotbar.Slots.ToList();
			mainPlayerSlots.AddRange(popupSlots);
			MainPlayerSlots = mainPlayerSlots.ToArray();
			hotbar.SetActiveSlot(0);

			inputHandler.RegisterInputEvent(inputHandler.GetAction("Change Hotbar Slot"), pausable: true, binding => {
				binding.Performed(context => {
					int keySlot = int.Parse(context.control.name);
					Hotbar.SetActiveSlot(keySlot - 1);
				});
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("Scroll Hotbar Slot"), pausable: true, binding => {
				binding.Performed(context => {
					float scrollDelta = context.ReadValue<float>();
					Hotbar.OnHotbarScroll(scrollDelta);
				});
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("Drop Item"), pausable: true, binding => {
				binding.Performed(_ => DropHoveredOrActiveItem());
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("Toggle Inventory"), pausable: true, binding => {
				binding.Performed(_ => ToggleInventory());
			});
			inputHandler.RegisterInputEvent(inputHandler.GetAction("LeftClick"), pausable: true, binding => {
				binding.Performed(_ => leftHold = true);
				binding.Canceled(_ => leftHold = false);
			});
		}

		public void SetupGrid() {
			InventorySlot[] flatPopupSlots = popup.GetComponentsInChildren<InventorySlot>(true);
			for (int i = 0; i < flatPopupSlots.Length; i++) {
				flatPopupSlots[i].index = i;
				popupSlotsByIndex[i] = flatPopupSlots[i];
			}
			popupSlots = ArrayHelper.CompressTo2D(flatPopupSlots, rows, columns);
			ArmorSlot[] armorSlots = this.armorSlots.GetComponentsInChildren<ArmorSlot>(true);
			for (int i = 0; i < armorSlots.Length; i++) {
				armorSlots[i].index = i;
				armorSlotsByIndex[i] = armorSlots[i];
			}
		}

		public SerializedInventory Serialize() {
			if (GrabbedContext.value != null) {
				TryReleaseGrabbed(GrabbedContext.lastKnownSlot);
			}
			List<SerializedItemSlot> Serialize<TSlot>(TSlot[] slots) where TSlot : IItemSlot {
				return slots.Cast<IItemSlot>().Select(s => s.Serialize()).ToList();
			}
			ArmorSlot[] armorSlots = this.armorSlots.GetComponentsInChildren<ArmorSlot>(true);

			return new SerializedInventory(hotbar.ActiveKey, new Dictionary<string, List<SerializedItemSlot>>() {
				["hotbar"] = Serialize(hotbar.Slots),
				["popup"] = Serialize(popupSlots.Flatten()),
				["armor"] = Serialize(armorSlots)
			});
		}

		public void Deserialize(SerializedInventory serialized) {
			var pendingUpdates = TryDeserialize("hotbar", serialized.regions, hotbar.GetSlotByIndex)
				?.Concat(TryDeserialize("popup", serialized.regions, this.GetSlotByIndex))
				?.Concat(TryDeserialize("armor", serialized.regions, index => armorSlotsByIndex[index]));
			hotbar.SetActiveSlot(serialized.lastHotbarIndex);

			this.slots[0].CreateDisplay(new ItemStack(Items.statItem_test, 1));

			pendingUpdates?.ToList().ForEach(s => s.item?.OnAttachedInSlot(s));
		}

		private IItemSlot[] TryDeserialize<TSlot>(string region, Dictionary<string, List<SerializedItemSlot>> regions, Func<int, TSlot> slotSupplier)
					where TSlot : MonoBehaviour, IItemSlot {
			List<TSlot> pendingAttachUpdates = new();

			if (!regions.TryGetValue(region, out var serializedRegion)) {
				logger.LogError(null, "Inventory region not found: {}", region);
				return null!;
			}
			foreach (var serializedSlot in serializedRegion) {
				if (serializedSlot.itemStack != null) {
					TSlot slot = slotSupplier.Invoke(serializedSlot.index);
					slot.Deserialize(serializedSlot);
					pendingAttachUpdates.Add(slot);
				}
			}
			return pendingAttachUpdates.ToArray();
		}

		private void PreInitState() {
			armorSlots.SetActive(opened);
			popup.SetActive(opened);
		}

		private void ToggleInventory() {
			opened = !opened;
			popup.SetActive(opened);
			armorSlots.SetActive(opened);
			LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
			hotbar.OnInventoryPopup();
			if (!opened && GrabbedContext.value != null) {
				TryReleaseGrabbed(GrabbedContext.lastKnownSlot);
			}
			foreach (var slot in popupSlots) {
				slot.OnInventoryPopup(opened);
			}
		}

		void IItemContainer.OnItemDisplayAdded(ItemDisplay itemDisplay, IItemSlot slot) {
			itemDisplay.onDestroy += player.OnItemDisplayDestroyed;

			InventorySlot hotbarSlot = itemDisplay.GetComponentInParent<InventorySlot>();
			if (!this.IsOpened && hotbarSlot != null && Hotbar.ActiveSlot != hotbarSlot) {
				itemDisplay.stackText.GetComponent<TextMeshProUGUI>().color = Hotbar.inactiveSlotNumberColor;
			}
		}

		private void DropHoveredOrActiveItem() {
			IItemSlot? slot = hoveredSlot ?? hotbar.ActiveSlot ?? null;
			if (slot == null) {
				return;
			}
			ItemDisplay hoveredDisplay = slot.itemDisplay;
			slot.DetachItemDisplay(this.transform);
			hoveredDisplay.stack.Drop(player.center, player.itemDropForce, true);
			hoveredDisplay.Destroy();
		}

		// POTENTIAL: OnDrop callback in Item

		public void DropGrabbedItem() {
			GrabbedContext.value?.stack.Drop(player.center, player.itemDropForce, true);
			GrabbedContext.value?.Destroy();
			if (GrabbedContext.value?.stack == player.MainHandStack) {
				player.SetMainHandItem(null!);
			}
			GrabbedContext.Set(null, null);
		}

		public void ForceReleaseGrabbed() {
			if (GrabbedContext.value == null) {
				return;
			}
			if (PickUpItem(GrabbedContext.value.stack, out int remaining)) {
				GrabbedContext.value.Destroy();
				GrabbedContext.Set(null, null);
			} else {
				GrabbedContext.value.stack.SetQuantity(remaining);
				DropGrabbedItem();
			}
		}

		private void TryReleaseGrabbed(IItemSlot? slot) {
			if (slot != null) {
				ExecuteOnGrabbedReference(slot, (slot, grabbedItem) => {
					InvocationHelper.If(!MergeInSlot(slot!, grabbedItem), ForceReleaseGrabbed);
				});
			} else {
				ForceReleaseGrabbed();
			}
		}

		public bool PickUpItem(ItemStack itemStack, out int remaining) {
			if (!itemStack.item.IsStackable) {
				InventorySlot? emptySlot = GetFirstEmptySlot();
				if (emptySlot != null) {
					(emptySlot as IItemSlot).CreateDisplay(itemStack);
					itemStack.OnPickedUp();
					remaining = 0;
					return true;
				}
				remaining = 1;
				return false;
			} else {
				void ClampRemaining(ref int remaining) => remaining = Mathf.Max(remaining, 0);
				remaining = itemStack.quantity;

				// Search for stack with the same item
				foreach (var stackSlot in GetOccupiedSlots(itemStack.item) ?? new List<IItemSlot>().ToArray()) {
					ItemStack? stackInSlot = stackSlot.stack;
					if (stackInSlot!.quantity < itemStack.item.maxStackSize) {
						int availableSpace = itemStack.item.maxStackSize - stackInSlot.quantity;
						int toAdd = Math.Min(remaining, availableSpace);
						stackInSlot.Increment(toAdd);
						remaining -= toAdd;
						ClampRemaining(ref remaining);
						if (remaining == 0) {
							itemStack.OnPickedUp();
							return true;
						}
					}
				}

				// Flow to empty slots
				InventorySlot[] availableSlots = MainPlayerSlots;
				for (int i = 0; i < availableSlots.Length; i++) {
					if (!availableSlots[i].HasItem) {
						int toAdd = Mathf.Min(remaining, itemStack.item.maxStackSize);
						(availableSlots[i] as IItemSlot).CreateDisplay(new ItemStack(itemStack.item, toAdd));
						remaining -= toAdd;
					}
					ClampRemaining(ref remaining);
					if (remaining == 0) {
						itemStack.OnPickedUp();
						return true;
					}
				}
				return remaining <= 0;
			}
		}

		public InventorySlot? GetFirstEmptySlot() => MainPlayerSlots.First(slot => slot.IsEmpty) ?? null;

		public InventorySlot[]? GetOccupiedSlots() => MainPlayerSlots.Where(slot => slot.HasItem)?.ToArray();

		public InventorySlot[]? GetOccupiedSlots(Item item) => GetOccupiedSlots().Where(slot => slot.stack.item.Equals(item))?.ToArray();

		public InventorySlot[]? GetEmptySlots() => MainPlayerSlots.Where(slot => !slot.HasItem)?.ToArray();

		public void EquipHotbarItem(InventorySlot slot) {
			ItemDisplay? itemDisplay = slot.itemDisplay;
			player.SetMainHandItem(itemDisplay?.stack!);
		}

		public InventorySlot GetSlotByIndex(int index) => popupSlotsByIndex[index];
		IItemSlot IItemContainer.GetSlotByIndex(int index) => popupSlotsByIndex[index];

		public void OnPointerDown(IItemSlot clickedSlot, PointerEventData eventData) {
			float time = Time.time;
			bool doubleClick = lastClickedSlot == clickedSlot && (time - lastClickTime) <= doubleClickThreshold;
			lastClickTime = time;
			lastClickedSlot = clickedSlot;

			PointerEventData.InputButton dragButton = eventData.button;
			InterpretationFunction? interpretationFunction = InterpretClick(clickedSlot, dragButton, doubleClick, out bool cancelDrag);
			if (interpretationFunction == null) {
				logger.ThrowException(null, new InvalidOperationException("InterpretClick() returned null!"));
				return;
			}

			actionResolver.Submit(Request.New()
				.UnderToken(PlayerActionTokens.SlotClick)
				.Execute(() => {
					ExecuteOnGrabbedReference(clickedSlot, (slot, grabbedReference) => {
						interpretationFunction.Invoke(slot!, grabbedReference);
						hotbar.OnItemTransfer(slot!, grabbedReference);
						if (GrabbedContext.value != null && !doubleClick && !cancelDrag) {
							activeDragHandler = this.StartDrag(slot!, dragButton);
						}
					});
				})
				.OnCondition(() => clickedSlot.Handshake(GrabbedContext.value, SlotInteractionMode.Click))
				.WithPriority(PlayerActionTokens.SlotClick.effectivePriority)
				.Suppress(PlayerActionTokens.ItemUse, () => !leftHold)
				.Suppress(PlayerActionTokens.Attack,() => !leftHold)
			);
		}

		public void OnPointerUp(IItemSlot slot, PointerEventData eventData) {
			this.EndDrag();
		}

		public void OnPointerEnter(IItemSlot slot, PointerEventData eventData) {
			this.hoveredSlot = slot;
			if (activeDragHandler == null || !slot.Handshake(GrabbedContext.value, SlotInteractionMode.Drag)) {
				return;
			}
			RefBox<ItemDisplay> grabbedReference = new(GrabbedContext.value);
			if (!this.activeDragHandler.ExecuteInterpretation(slot, grabbedReference)) {
				logger.ThrowException(null, new InvalidOperationException("Drag interpretation function returned null!"));
				return;
			}
			hotbar.OnItemTransfer(slot, grabbedReference);
			this.GrabbedContext.Set(grabbedReference.value, null);
		}

		public void OnPointerExit(IItemSlot slot, PointerEventData eventData) {
			this.hoveredSlot = null;
		}

		public InterpretationFunction? InterpretClick(IItemSlot clickedSlot, PointerEventData.InputButton button, bool doubleClick, out bool cancelDrag) {
			bool shift = Keyboard.current.shiftKey.isPressed;
			bool ctrl = Keyboard.current.ctrlKey.isPressed;
			bool alt = Keyboard.current.altKey.isPressed;

			if (button == PointerEventData.InputButton.Left) {
				if (doubleClick && GrabbedContext.value != null) {
					cancelDrag = false;
					return CollectAllStacksInGrabbed_Impl;
				}
				cancelDrag = false;
				return TransferGrabbed;
			} else if (button == PointerEventData.InputButton.Right) {
				if (GrabbedContext.value != null && clickedSlot.item != null) {
					if (GrabbedContext.value?.item != clickedSlot.itemDisplay?.item) {
						cancelDrag = false;
						return DoNothing;
					}
				}
				if (GrabbedContext.value != null) {
					cancelDrag = false;
					return TransferSingleToSlot;
				}
				cancelDrag = false;
				return HalveStackFromSlot;
			}
			cancelDrag = false;
			return null;
		}

		public InterpretationFunction? InterpretDrag(DragHandler handler, IItemSlot draggedSlot, RefBox<ItemDisplay> grabbedItem) {
			if (handler.dragButton == PointerEventData.InputButton.Left) {
				return SplitDistributeToDraggedSlot_Impl;
			} else if (handler.dragButton == PointerEventData.InputButton.Right) {
				if (grabbedItem.value != null && draggedSlot.item != null) {
					if (grabbedItem.value.item != draggedSlot.item) {
						return DoNothing;
					}
				}
				return TransferSingleToSlot;
			}

			return null;
		}

		public DragHandler StartDrag(IItemSlot dragOrigin, PointerEventData.InputButton dragButton) {
			Dictionary<IItemSlot, int> quantitySnapshots = new();
			var snapshotSlots = slots.Where(s => s != dragOrigin && s.item == dragOrigin.item);
			foreach (var slot in snapshotSlots) {
				quantitySnapshots[slot] = slot.stack?.quantity ?? 0;
			}
			return new DragHandler(dragOrigin, () => slots.ToArray(), this.InterpretDrag, quantitySnapshots, dragButton);
		}

		public void EndDrag() {
			activeDragHandler?.OnDragEnd();
			activeDragHandler = null;
		}

		private void ExecuteOnGrabbedReference(IItemSlot? slot, Action<IItemSlot?, RefBox<ItemDisplay>> referenceAction) {
			RefBox<ItemDisplay> grabbedReference = new(GrabbedContext.value);
			referenceAction.Invoke(slot, grabbedReference);
			GrabbedContext.Set(grabbedReference.value, slot);
		}

		[InterpretationFunctionCandidate]
		public void DoNothing(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			// Intended implementation
		}

		[InterpretationFunctionCandidate]
		public void CollectAllStacksInGrabbed(Item item, RefBox<ItemDisplay> grabbedItem) {
			if (grabbedItem.value == null || grabbedItem.value.stack.IsFull()) {
				return;
			}

			var slots = GetOccupiedSlots(item)?.OrderBy(slot => slot.stack.quantity).ToList();
			if (slots == null || slots.Count == 0) {
				return;
			}

			ItemStack grabbedStack = grabbedItem.value.stack;
			int spaceLeft = item.maxStackSize - grabbedStack.quantity;
			foreach (var slot in slots) {
				if (spaceLeft <= 0) {
					break;
				}

				int transfer = grabbedStack.Increment(slot.stack.quantity);
				slot.stack.Decrement(transfer);
				spaceLeft -= transfer;
			}
		}

		public void CollectAllStacksInGrabbed_Impl(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			CollectAllStacksInGrabbed(grabbedItem.value?.stack.item
				?? throw new InvalidOperationException("Grabbed context not available"), grabbedItem);
		}

		[InterpretationFunctionCandidate]
		public void HalveStackFromSlot(IItemSlot clickedSlot, RefBox<ItemDisplay> grabbedItem) {
			if (clickedSlot.stack == null || grabbedItem.value != null) {
				return;
			}
			int half = clickedSlot.stack.quantity / 2;
			int remainder = clickedSlot.stack.quantity % 2;
			int transfer = half + remainder;
			clickedSlot.stack.Decrement(transfer);
			CreateGrabbedDisplay(grabbedItem, new ItemStack(clickedSlot.stack.item, transfer));
		}

		[InterpretationFunctionCandidate]
		public void TransferSingleToSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (grabbedItem.value == null) {
				return;
			}
			int added = slot.TryAddStack(1, grabbedItem.value.stack.item);
			if (added > 0) {
				grabbedItem.value.stack.Decrement(added);
				DestroyGrabbedIfEmpty(grabbedItem);
			}
		}

		[InterpretationFunctionCandidate]
		public void TransferSingleToGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (slot.stack == null) {
				return;
			}
			this.AddToGrabbed(slot, grabbedItem, 1);
		}

		[InterpretationFunctionCandidate]
		public void AddToGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem, int amount) {
			if (slot.stack == null) {
				return;
			}
			if (grabbedItem.value == null) {
				CreateGrabbedDisplay(grabbedItem, new ItemStack(slot.stack.item, 0));
			}
			int transfer = Mathf.Min(amount, slot.stack.quantity);
			slot.stack.Decrement(transfer);
			grabbedItem.value!.stack.Increment(transfer);
		}

		[InterpretationFunctionCandidate]
		public void TransferGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (!IsOpened) {
				return;
			}
			PlayerController player = Soulbound.instance.GetPlayerInstance();
			void SetRightClickAvailable(bool enabled) {
				Action<ItemUseTrigger[]> action = enabled ? player.ItemUsageHandler.Enable : player.ItemUsageHandler.Disable;
				action.Invoke(new ItemUseTrigger[] { ItemUseTrigger.RightClick, ItemUseTrigger.RightHold });
			}

			// Grab if slot has item, grabbed is empty
			if (grabbedItem.value == null && slot.stack != null) {
				SetRightClickAvailable(false);
				this.GrabItemFromSlot(slot, grabbedItem);
				return;
			}

			// Release if slot is empty, grabbed has item
			if (grabbedItem.value != null && slot.stack == null) {
				SetRightClickAvailable(true);
				this.ReleaseItemInSlot(slot, grabbedItem);
				return;
			}

			// Swap if different items or max quantity exists in either stack
			if (slot.item != grabbedItem.value!.stack.item || grabbedItem.value.stack.IsFull() || slot.stack!.IsFull()) {
				this.SwapItems(slot, grabbedItem);
			} else {    // Merge in slot for compatible stacks
				SetRightClickAvailable(MergeInSlot(slot, grabbedItem));
			}
		}

		[InterpretationFunctionCandidate]
		public void GrabItemFromSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (slot.IsEmpty || grabbedItem.value != null) {
				return;
			}
			grabbedItem.value = slot.itemDisplay;
			slot.DetachItemDisplay(this.transform);
			player.SetMainHandItem(grabbedItem.value.stack);
		}

		[InterpretationFunctionCandidate]
		public void ReleaseItemInSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (slot.HasItem || grabbedItem.value == null) {
				return;
			}
			(slot as IItemSlot).AttachItemDisplay(grabbedItem.value);
			player.SetMainHandItem(Hotbar.ActiveSlot.stack);
			grabbedItem.value = null;
		}

		[InterpretationFunctionCandidate]
		public void SwapItems(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (slot.IsEmpty || grabbedItem.value == null) {
				return;
			}
			ItemDisplay previousGrabbed = grabbedItem.value;
			grabbedItem.value = slot.itemDisplay;
			slot.DetachItemDisplay(this.transform);
			slot.AttachItemDisplay(previousGrabbed);
			player.SetMainHandItem(grabbedItem.value.stack);
		}

		[InterpretationFunctionCandidate("Attempts to merge the grabbed stack into the clicked slot. Returns true if fully merged")]
		public bool MergeInSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (grabbedItem.value == null) {
				return false;
			}
			if (slot.IsEmpty) {
				this.ReleaseItemInSlot(slot, grabbedItem);
				return true;
			}
			if (slot.stack!.item != grabbedItem.value.stack.item) {
				return false;
			}

			int space = slot.stack!.item.maxStackSize - slot.stack.quantity;
			if (space < 0) {
				return false;
			}
			int transfer = Math.Min(space, grabbedItem.value.stack.quantity);
			slot.stack.Increment(transfer);
			grabbedItem.value.stack.Decrement(transfer);
			return DestroyGrabbedIfEmpty(grabbedItem);
		}

		public void SplitDistributeToDraggedSlot_Impl(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			SplitDistributeToDraggedSlot(this.activeDragHandler
				?? throw new InvalidOperationException("Drag context does not exist"), slot, grabbedItem);
		}

		public void SplitDistributeToDraggedSlot(DragHandler handler, IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (handler.DraggedSlots.Contains(slot)
					|| (slot.HasItem && slot.item != handler.origin!.item)
					|| (slot.HasItem && slot.stack!.IsFull())) {
				return;
			}

			// Clone to preview distribution
			List<IItemSlot> preview = new(handler.DraggedSlots) { slot };

			int toSplit = handler.startDragStack;
			int splitAmount = toSplit / (preview.Count);
			if (splitAmount <= 0) {
				return;
			}
			// Commit the slot to dragged list
			handler.AddDraggedSlot(slot);
			int remainder = toSplit % (handler.DraggedSlots.Count);

			for (int i = 0; i < handler.DraggedSlots.Count; i++) {
				var draggedSlot = handler.DraggedSlots[i];
				int amount = splitAmount + (i < remainder ? 1 : 0);

				if (draggedSlot.stack == null) {
					draggedSlot.CreateDisplay(new ItemStack(handler.origin!.stack!.item, amount));
				}
				if (handler.quantitySnapshots.TryGetValue(draggedSlot, out var snapshot) && draggedSlot != handler.origin) {
					draggedSlot.stack!.SetQuantity(snapshot + amount);
				} else {
					draggedSlot.stack!.SetQuantity(amount);
				}
			}
		}

		public ItemDisplay CreateGrabbedDisplay(ItemStack itemStack, Func<Transform?>? parentSupplier = null) {
			ItemDisplay grabbed = ItemDisplay.Create(itemStack, parentSupplier ?? (() => this.transform));
			grabbed.OnGrab(this.transform, true);
			return grabbed;
		}

		public void CreateGrabbedDisplay(RefBox<ItemDisplay> grabbedReference, ItemStack itemStack, Func<Transform?>? parentSupplier = null) {
			ItemDisplay grabbed = CreateGrabbedDisplay(itemStack, parentSupplier);
			grabbedReference.value = grabbed;
		}

		public RefBox<ItemDisplay> CreateGrabbedReference(ItemStack itemStack, Func<Transform?>? parentSupplier = null) {
			RefBox<ItemDisplay> grabbedReference = new(CreateGrabbedDisplay(itemStack, parentSupplier));
			return grabbedReference;
		}

		private bool DestroyGrabbedIfEmpty(RefBox<ItemDisplay> grabbedItem) {
			if (grabbedItem.value == null) {
				return false;
			}
			if (grabbedItem.value.stack.IsEmpty()) {
				grabbedItem.value.Destroy();
				grabbedItem.value = null;
				return true;
			}
			return false;
		}
	}
}
