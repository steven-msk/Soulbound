using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	[BootstrappableChildOf(typeof(PlayerController))]
	[BootstrappableParentOf(typeof(HotbarController))]
    public class InventoryController : MonoBehaviour, IItemContainer2D, IBootstrappable, ISerializable<SerializedInventory, List<IItemSlot>> {
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

		private PlayerController player;
		public InventoryEventHandler eventHandler { get; private set; }

		private float doubleClickThreshold = 0.15f;
		private float lastClickTime;
		private IItemSlot? lastClickedSlot;
		private DragHandler? activeDragHandler;
		private IItemSlot? hoveredSlot;

		public void OnBootstrap(DependencyContainer dependencyContainer) {
			this.eventHandler = new InventoryEventHandler();
			player = dependencyContainer.Resolve<PlayerController>();
			hotbar.OnBootstrap(dependencyContainer);
			popup.SetActive(false);
			armorSlots.SetActive(false);
			this.SetupGrid();

			List<InventorySlot> mainPlayerSlots = hotbar.Slots.ToList();
			mainPlayerSlots.AddRange(popupSlots);
			MainPlayerSlots = mainPlayerSlots.ToArray();
			hotbar.SetActiveSlot(0);
        }

		public void OnEarlyBootstrap(DependencyContainer dependencyContainer) {
			dependencyContainer.Register<InventoryController>(this);
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

		public List<IItemSlot> Deserialize(SerializedInventory serialized) {
			var pendingUpdates = TryDeserialize("hotbar", serialized.regions, hotbar.GetSlotByIndex)
				?.Concat(TryDeserialize("popup", serialized.regions, this.GetSlotByIndex))
				?.Concat(TryDeserialize("armor", serialized.regions, index => armorSlotsByIndex[index]));
			hotbar.SetActiveSlot(serialized.lastHotbarIndex);

			//ItemDisplay.Create(new ItemStack(Items.toolItem_test, 1), this.slots[0], this);

			return pendingUpdates?.ToList()!;
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

		public void ToggleInventory() {
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

		public void DropHoveredOrActiveItem() {
			IItemSlot? slot = hoveredSlot ?? hotbar.ActiveSlot ?? null;
			if (slot == null) {
				return;
			}
			ItemDisplay hoveredDisplay = slot.ItemDisplay;
			slot.DetachItemDisplay(this.transform);
			hoveredDisplay.ItemStack.Drop(player.center, player.itemDropForce, true);
			hoveredDisplay.Destroy();
		}

		// POTENTIAL: OnDrop callback in Item

		public void DropGrabbedItem() {
			GrabbedContext.value?.ItemStack.Drop(player.center, player.itemDropForce, true);
			GrabbedContext.value?.Destroy();
			if (GrabbedContext.value?.ItemStack == player.MainHandStack) {
				player.SetMainHandItem(null!);
			}
			GrabbedContext.Set(null, null);
		}

		public void ForceReleaseGrabbed() {
			if (GrabbedContext.value == null) {
				return;
			}
			if (PickUpItem(GrabbedContext.value.ItemStack, out int remaining)) {
				GrabbedContext.value.Destroy();
				GrabbedContext.Set(null, null);
			} else {
				GrabbedContext.value.ItemStack.SetQuantity(remaining);
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
					ItemStack? stackInSlot = stackSlot.ItemStack;
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

		public InventorySlot[]? GetOccupiedSlots(Item item) => GetOccupiedSlots().Where(slot => slot.ItemStack.item.Equals(item))?.ToArray();

		public InventorySlot[]? GetEmptySlots() => MainPlayerSlots.Where(slot => !slot.HasItem)?.ToArray();

		public void EquipHotbarItem(InventorySlot slot) {
			ItemDisplay? itemDisplay = slot.ItemDisplay;
			player.SetMainHandItem(itemDisplay?.ItemStack!);
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

			InputHandler.RequestAction(new("ItemSlotAction", 10, () => {
				if (!clickedSlot.Handshake(GrabbedContext.value, SlotInteractionMode.Click)) {
					return;
				}
				ExecuteOnGrabbedReference(clickedSlot, (slot, grabbedReference) => {
					interpretationFunction.Invoke(slot!, grabbedReference);
					hotbar.OnItemTransfer(slot!, grabbedReference);
					if (GrabbedContext.value != null && !doubleClick && !cancelDrag) {
						activeDragHandler = this.StartDrag(slot!, dragButton);
					}
				});
			}, null));
			InputHandler.BlockContext("ItemUse", () => !Soulbound.instance.GetActiveLevel()!.Player.InputHandler.LeftHold);
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
				if (GrabbedContext.value != null && clickedSlot.ContainedItem != null) {
					if (GrabbedContext.value?.DisplayedItem != clickedSlot.ItemDisplay?.DisplayedItem) {
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
				if (grabbedItem.value != null && draggedSlot.ContainedItem != null) {
					if (grabbedItem.value.DisplayedItem != draggedSlot.ContainedItem) {
						return DoNothing;
					}
				}
				return TransferSingleToSlot;
			}

			return null;
		}

		public DragHandler StartDrag(IItemSlot dragOrigin, PointerEventData.InputButton dragButton) {
			Dictionary<IItemSlot, int> quantitySnapshots = new();
			var snapshotSlots = slots.Where(s => s != dragOrigin && s.ContainedItem == dragOrigin.ContainedItem);
			foreach (var slot in snapshotSlots) {
				quantitySnapshots[slot] = slot.ItemStack?.quantity ?? 0;
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
			if (grabbedItem.value == null || grabbedItem.value.ItemStack.IsFull()) {
				return;
			}

			var slots = GetOccupiedSlots(item)?.OrderBy(slot => slot.ItemStack.quantity).ToList();
			if (slots == null || slots.Count == 0) {
				return;
			}

			ItemStack grabbedStack = grabbedItem.value.ItemStack;
			int spaceLeft = item.maxStackSize - grabbedStack.quantity;
			foreach (var slot in slots) {
				if (spaceLeft <= 0) {
					break;
				}

				int transfer = grabbedStack.Increment(slot.ItemStack.quantity);
				slot.ItemStack.Decrement(transfer);
				spaceLeft -= transfer;
			}
		}

		public void CollectAllStacksInGrabbed_Impl(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			CollectAllStacksInGrabbed(grabbedItem.value?.ItemStack.item
				?? throw new InvalidOperationException("Grabbed context not available"), grabbedItem);
		}

		[InterpretationFunctionCandidate]
		public void HalveStackFromSlot(IItemSlot clickedSlot, RefBox<ItemDisplay> grabbedItem) {
			if (clickedSlot.ItemStack == null || grabbedItem.value != null) {
				return;
			}
			int half = clickedSlot.ItemStack.quantity / 2;
			int remainder = clickedSlot.ItemStack.quantity % 2;
			int transfer = half + remainder;
			clickedSlot.ItemStack.Decrement(transfer);
			CreateGrabbedDisplay(grabbedItem, new ItemStack(clickedSlot.ItemStack.item, transfer));
		}

		[InterpretationFunctionCandidate]
		public void TransferSingleToSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (grabbedItem.value == null) {
				return;
			}
			int added = slot.TryAddStack(1, grabbedItem.value.ItemStack.item);
			if (added > 0) {
				grabbedItem.value.ItemStack.Decrement(added);
				DestroyGrabbedIfEmpty(grabbedItem);
			}
		}

		[InterpretationFunctionCandidate]
		public void TransferSingleToGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (slot.ItemStack == null) {
				return;
			}
			this.AddToGrabbed(slot, grabbedItem, 1);
		}

		[InterpretationFunctionCandidate]
		public void AddToGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem, int amount) {
			if (slot.ItemStack == null) {
				return;
			}
			if (grabbedItem.value == null) {
				CreateGrabbedDisplay(grabbedItem, new ItemStack(slot.ItemStack.item, 0));
			}
			int transfer = Mathf.Min(amount, slot.ItemStack.quantity);
			slot.ItemStack.Decrement(transfer);
			grabbedItem.value!.ItemStack.Increment(transfer);
		}

		[InterpretationFunctionCandidate]
		public void TransferGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (!IsOpened) {
				return;
			}
			PlayerController player = Soulbound.instance.GetActiveLevel()!.Player;
			void SetRightClickAvailable(bool enabled) {
				Action<ItemUseTrigger[]> action = enabled ? player.ItemUsageHandler.Enable : player.ItemUsageHandler.Disable;
				action.Invoke(new ItemUseTrigger[] { ItemUseTrigger.RightClick, ItemUseTrigger.RightHold });
			}

			// Grab if slot has item, grabbed is empty
			if (grabbedItem.value == null && slot.ItemStack != null) {
				SetRightClickAvailable(false);
				this.GrabItemFromSlot(slot, grabbedItem);
				return;
			}

			// Release if slot is empty, grabbed has item
			if (grabbedItem.value != null && slot.ItemStack == null) {
				SetRightClickAvailable(true);
				this.ReleaseItemInSlot(slot, grabbedItem);
				return;
			}

			// Swap if different items or max quantity exists in either stack
			if (slot.ContainedItem != grabbedItem.value!.ItemStack.item || grabbedItem.value.ItemStack.IsFull() || slot.ItemStack!.IsFull()) {
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
			grabbedItem.value = slot.ItemDisplay;
			slot.DetachItemDisplay(this.transform);
			player.SetMainHandItem(grabbedItem.value.ItemStack);
		}

		[InterpretationFunctionCandidate]
		public void ReleaseItemInSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (slot.HasItem || grabbedItem.value == null) {
				return;
			}
			(slot as IItemSlot).AttachItemDisplay(grabbedItem.value);
			player.SetMainHandItem(Hotbar.ActiveSlot.ItemStack);
			grabbedItem.value = null;
		}

		[InterpretationFunctionCandidate]
		public void SwapItems(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (slot.IsEmpty || grabbedItem.value == null) {
				return;
			}
			ItemDisplay previousGrabbed = grabbedItem.value;
			grabbedItem.value = slot.ItemDisplay;
			slot.DetachItemDisplay(this.transform);
			slot.AttachItemDisplay(previousGrabbed);
			player.SetMainHandItem(grabbedItem.value.ItemStack);
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
			if (slot.ItemStack!.item != grabbedItem.value.ItemStack.item) {
				return false;
			}

			int space = slot.ItemStack!.item.maxStackSize - slot.ItemStack.quantity;
			if (space < 0) {
				return false;
			}
			int transfer = Math.Min(space, grabbedItem.value.ItemStack.quantity);
			slot.ItemStack.Increment(transfer);
			grabbedItem.value.ItemStack.Decrement(transfer);
			return DestroyGrabbedIfEmpty(grabbedItem);
		}

		public void SplitDistributeToDraggedSlot_Impl(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			SplitDistributeToDraggedSlot(this.activeDragHandler
				?? throw new InvalidOperationException("Drag context does not exist"), slot, grabbedItem);
		}

		public void SplitDistributeToDraggedSlot(DragHandler handler, IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			if (handler.DraggedSlots.Contains(slot)
					|| (slot.HasItem && slot.ContainedItem != handler.origin!.ContainedItem)
					|| (slot.HasItem && slot.ItemStack!.IsFull())) {
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

				if (draggedSlot.ItemStack == null) {
					draggedSlot.CreateDisplay(new ItemStack(handler.origin!.ItemStack!.item, amount));
				}
				if (handler.quantitySnapshots.TryGetValue(draggedSlot, out var snapshot) && draggedSlot != handler.origin) {
					draggedSlot.ItemStack!.SetQuantity(snapshot + amount);
				} else {
					draggedSlot.ItemStack!.SetQuantity(amount);
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
			if (grabbedItem.value.ItemStack.IsEmpty()) {
				grabbedItem.value.Destroy();
				grabbedItem.value = null;
				return true;
			}
			return false;
		}
	}
}
