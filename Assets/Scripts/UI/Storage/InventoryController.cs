using JetBrains.Annotations;
using PlasticGui.WorkspaceWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

#nullable enable
public class InventoryController : MonoBehaviour, IItemContainer2D, IDependencyInitializable<InventoryController, PlayerController>, ISerializable<SerializedInventory> {
	public delegate void InterpretationFunction(IItemSlot clickedSlot, RefBox<ItemDisplay> grabbedItem);

	private static readonly Logger logger = Logger.CreateInstance();
	public HotbarController Hotbar => hotbar;
	[SerializeField] private HotbarController hotbar;

	[SerializeField] private GameObject popup;
	[SerializeField] private GameObject armorSlots;
	private static Dictionary<int, ArmorSlot> armorSlotsByIndex = new();
	[SerializeField] private bool popupOpen = false;

	public bool PopupOpen => popupOpen;
	public ItemDisplay? GrabbedItem { get; set; }

	public AbstractTooltip ActiveTooltip { set => activeTooltip = value; }
	[SerializeField] private AbstractTooltip activeTooltip;

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

	private bool draggingSlots;
	private List<IItemSlot> draggedSlots = new();
	private IItemSlot? dragOrigin;
	private int startDragStack;
	private Dictionary<IItemSlot, int> quantitySnapshots = new();
	private PointerEventData.InputButton dragButton;
	private float doubleClickThreshold = 0.2f;
	private float lastClickTime;
	private IItemSlot lastClickedSlot;

	// FEATUREIMPL: item grabbing controls - this might require a general implementation in IContainer

	public InventoryController OnGameInit(PlayerController dependency) {
		player = dependency;
		hotbar.OnGameInit(this);
		popup.SetActive(false);
		armorSlots.SetActive(false);
		this.SetupGrid();

		// There seems to be a weird occlusion where inventory children get enabled at instantiation,
		// though in the prefab they are not. There are no SetActive(true) calls either.

		List<InventorySlot> mainPlayerSlots = hotbar.Slots.ToList();
		mainPlayerSlots.AddRange(popupSlots);
		MainPlayerSlots = mainPlayerSlots.ToArray();
		hotbar.SetActiveSlot(0);
		return this;
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
		var serializedHotbar = hotbar.Slots.Cast<IItemSlot>().Select(s => s.Serialize()).ToList();
		var serializedPopup = popupSlots.Flatten().Cast<IItemSlot>().Select(s => s.Serialize()).ToList();
		ArmorSlot[] armorSlots = this.armorSlots.GetComponentsInChildren<ArmorSlot>(true);
		var serializedArmor = armorSlots.Cast<IItemSlot>().Select(s => s.Serialize()).ToList();
		return new SerializedInventory(hotbar.ActiveKey, new Dictionary<string, List<SerializedItemSlot>>() {
			["hotbar"] = serializedHotbar, 
			["popup"] = serializedPopup,
			["armor"] = serializedArmor
		});
	}

	public void Deserialize(SerializedInventory serialized) {
		TryDeserialize("hotbar", serialized.regions, hotbar.GetSlotByIndex);
		TryDeserialize("popup", serialized.regions, this.GetSlotByIndex);
		TryDeserialize("armor", serialized.regions, (index) => armorSlotsByIndex[index]);
		hotbar.SetActiveSlot(serialized.lastHotbarIndex);
	}

	private void TryDeserialize<TSlot>(string region, Dictionary<string, List<SerializedItemSlot>> regions, Func<int, TSlot> slotSupplier)
				where TSlot : MonoBehaviour, IItemSlot {
		if (regions.TryGetValue(region, out var serializedRegion)) {
			serializedRegion.ForEach(serialized => {
				if (serialized.itemStack != null) {
					ItemDisplay.Create(serialized.itemStack, slotSupplier.Invoke(serialized.index));
				}
			});
		} else {
			logger.LogError(null, "Unknown inventory region: {}", region);
		}
	}

	private void Start() {
		foreach (var slot in MainPlayerSlots) {
			if (slot.IsEmpty) {
				ItemDisplay.Create(Items.grassBlock, 1, slot);
			}
		}
	}

	public void ToggleInventory() {
		popupOpen = !popupOpen;
		popup.SetActive(popupOpen);
		armorSlots.SetActive(popupOpen);
		LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>()); 
		activeTooltip?.Hide();
		hotbar.OnInventoryPopup();
		if (!popupOpen && GrabbedItem != null) {
			ForceReleaseGrabbedItem();
		}
	}

	public void DropItemFromInventory() {
		ItemDisplay itemDisplay = default(ItemDisplay)!;
		if (activeTooltip != null) {
			itemDisplay = activeTooltip.DisplayParent?.GetComponent<ItemDisplay>()!;
			activeTooltip.Hide();
			activeTooltip = null!;
		} else {
			itemDisplay = hotbar.ActiveSlot.ItemDisplay;
		}
		itemDisplay?.ItemStack.Drop(player.center, player.itemDropForce, true);
		Destroy(itemDisplay?.gameObject);
	}

	// POTENTIAL: OnDrop callback in Item

	public void DropGrabbedItem() {
		GrabbedItem?.ItemStack.Drop(player.center, player.itemDropForce, true);
		GrabbedItem?.Destroy();
		if (GrabbedItem?.ItemStack == player.MainHandStack) {
			player.SetMainHandItem(null!);
		}
		GrabbedItem = null;
	}

	public void ForceReleaseGrabbedItem() {
		if (GrabbedItem == null) {
			return;
		}
		if (PickUpItem(GrabbedItem.ItemStack)) {
			GrabbedItem.Destroy();
			GrabbedItem = null;
		} else {
			DropGrabbedItem();
		}
	}

	public bool PickUpItem(ItemStack itemStack) {
		if (!itemStack.item.IsStackable) {
			InventorySlot? emptySlot = GetFirstEmptySlot();
			if (emptySlot != null) {
				ItemDisplay.Create(itemStack, emptySlot);
				return true;
			}
			return false;
		} else {
			int remaining = itemStack.quantity;

			// Search for stack with the same item
			foreach (var stackSlot in GetOccupiedSlots(itemStack.item) ?? new List<IItemSlot>().ToArray()) {
				ItemStack? stackInSlot = stackSlot.ItemStack;
				if (stackInSlot!.quantity < stackInSlot!.item.maxStackSize) {
					int availableSpace = stackInSlot.item.maxStackSize - stackInSlot.quantity;
					int toAdd = Math.Min(remaining, availableSpace);
					stackInSlot.Increment(toAdd);
					remaining -= toAdd;
					if (remaining <= 0) {
						return true;
					}
				}
			}

			// Flow to empty slots
			InventorySlot[] availableSlots = MainPlayerSlots;
			for (int i = 0; i < availableSlots.Length; i++){
				if (!availableSlots[i].HasItem) {
					int toAdd = Mathf.Min(remaining, itemStack.item.maxStackSize);
					ItemDisplay.Create(new ItemStack(itemStack.item, toAdd), availableSlots[i]);
					remaining -= toAdd;
				}
				if (remaining <= 0) {
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
		ItemDisplay itemDisplay = slot.ItemDisplay;
		player.SetMainHandItem(itemDisplay?.ItemStack!);
	}

	public InventorySlot GetSlotByIndex(int index) => popupSlotsByIndex[index];
	IItemSlot IItemContainer.GetSlotByIndex(int index) => popupSlotsByIndex[index];

	public void OnPointerDown(IItemSlot slot, PointerEventData eventData) {
		float time = Time.time;
		bool doubleClick = lastClickedSlot == slot && (time - lastClickTime) <= doubleClickThreshold;
		lastClickTime = time;
		lastClickedSlot = slot;

		this.dragButton = eventData.button;
		InterpretationFunction? interpretationFunction = InterpretClick(slot, eventData, doubleClick, out bool cancelDrag);
		if (interpretationFunction == null) {
			logger.ThrowException(null, new InvalidOperationException("InterpretClick() returned null!"));
			return;
		}

		InputHandler.RequestAction(new("ItemSlotAction", 10, () => {
			if (!slot.Handshake(GrabbedItem)) {
				return;
			}
			RefBox<ItemDisplay> grabbedReference = new(this.GrabbedItem);
			interpretationFunction.Invoke(slot, grabbedReference);
			hotbar.OnItemTransfer(slot, grabbedReference);
			if (GrabbedItem != null && !doubleClick && !cancelDrag) {
				this.StartDrag(slot);
			}
			this.GrabbedItem = grabbedReference.value;
		}, null));
		InputHandler.BlockContext("ItemUse", () => !GameManager.instance.Player.InputHandler.LeftHold);
	}

	public void OnPointerUp(IItemSlot slot, PointerEventData eventData) { 
		InvocationHelper.If(draggingSlots, this.EndDrag);
	}

	public void OnPointerEnter(IItemSlot slot, PointerEventData eventData) {
		if (!draggingSlots || !slot.Handshake(GrabbedItem)) {
			return;
		}
		RefBox<ItemDisplay> grabbedReference = new(this.GrabbedItem);
		InterpretationFunction? interpretationFunction = this.InterpretDrag(slot, grabbedReference);
		if (interpretationFunction == null) {
			logger.ThrowException(null, new InvalidOperationException("InterpretDrag() returned null!"));
			return;
		}
		interpretationFunction.Invoke(slot, grabbedReference);
		hotbar.OnItemTransfer(slot, grabbedReference);
		this.GrabbedItem = grabbedReference.value;
	}

	public InterpretationFunction? InterpretClick(IItemSlot clickedSlot, PointerEventData eventData, bool doubleClick, out bool cancelDrag) {
		bool shift = Keyboard.current.shiftKey.isPressed;
		bool ctrl = Keyboard.current.ctrlKey.isPressed;
		bool alt = Keyboard.current.altKey.isPressed;

		if (eventData.button == PointerEventData.InputButton.Left) {
			if (doubleClick && GrabbedItem != null) {
				cancelDrag = false;
				return (slot, grabbedItem) => CollectAllStacksInGrabbed(grabbedItem.value!.ItemStack.item, grabbedItem);
			}
			cancelDrag = false;
			return TransferGrabbed;
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			if (GrabbedItem != null && clickedSlot.ContainedItem != null) {
				if (GrabbedItem?.DisplayedItem != clickedSlot.ItemDisplay?.DisplayedItem) {
					cancelDrag = false;
					return DoNothing;
				}
			}
			if (GrabbedItem != null) {
				cancelDrag = false;
				return TransferSingleToSlot;
			}
			cancelDrag = false;
			return HalveStackFromSlot;
		}
		cancelDrag = false;
		return null;
	}

	public InterpretationFunction? InterpretDrag(IItemSlot draggedSlot, RefBox<ItemDisplay> grabbedItem) {
		if (!draggingSlots) {
			logger.LogError(null, "InterpretDrag() called while not dragging slots. This should really not happen.");
			return null;
		}

		if (dragButton == PointerEventData.InputButton.Left) {
			return (slot, grabbedItem) => {
				if (draggedSlots.Contains(slot) 
						|| (slot.HasItem && slot.ContainedItem != dragOrigin!.ContainedItem)
						|| (slot.HasItem && slot.ItemStack!.IsFull())) {
					return;
				}

				// Clone to preview distribution
				List<IItemSlot> preview = new(draggedSlots) { slot };

				int toSplit = startDragStack;
				int splitAmount = toSplit / (preview.Count);
				if (splitAmount <= 0) {
					return;
				}
				// Commit the slot to dragged list
				draggedSlots.Add(slot);
				int remainder = toSplit % (draggedSlots.Count);

				for (int i = 0; i < draggedSlots.Count; i++) {
					var draggedSlot = draggedSlots[i];
					int amount = splitAmount + (i < remainder ? 1 : 0);

					if (draggedSlot.ItemStack == null) {
						draggedSlot.CreateDisplay(new ItemStack(dragOrigin!.ItemStack!.item, amount));
					}
					if (quantitySnapshots.TryGetValue(draggedSlot, out var snapshot) && draggedSlot != this.dragOrigin) {
						draggedSlot.ItemStack!.SetQuantity(snapshot + amount);
					} else {
						draggedSlot.ItemStack!.SetQuantity(amount);
					}
				}
			};
		} else if (dragButton == PointerEventData.InputButton.Right && grabbedItem.value?.ItemStack.quantity > 0) {
			if (draggedSlot.ContainedItem != null && grabbedItem.value.DisplayedItem != draggedSlot.ContainedItem) {
				return DoNothing;
			}
			return TransferSingleToSlot;
		}

		return null;
	}

	public void StartDrag(IItemSlot dragOrigin) {
		draggingSlots = true;
		this.dragOrigin = dragOrigin;
		this.startDragStack = dragOrigin.ItemStack!.quantity;
		quantitySnapshots.Clear();
		foreach (var slot in slots.Where(s => s != this.dragOrigin && s.ContainedItem == dragOrigin.ContainedItem)) {
			quantitySnapshots[slot] = slot.ItemStack!.quantity;
		}
		Debug.Log("starting drag: " + dragButton);
		draggedSlots.Add(dragOrigin);
	}

	public void EndDrag() {
		if (draggedSlots.Count == 0) {
			draggingSlots = false;
			this.dragOrigin = null;
			return;
		}

		draggingSlots = false;
		draggedSlots.Clear();
		this.dragOrigin = null;
	}

	[InterpretationFunctionCandidate]
	public void DoNothing(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
		// Intended implementation to remove "Interpret returned null!" spam
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
		Debug.Log("single to slot");
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
		Debug.Log("single to grabbed");
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

	public ItemDisplay CreateGrabbedDisplay(ItemStack itemStack, Func<Transform?>? parentSupplier = null) {
		ItemDisplay grabbed = ItemDisplay.Create(itemStack, parentSupplier ?? (() => this.transform));
		grabbed.EnableGrab();
		return grabbed;
	}

	public void CreateGrabbedDisplay(RefBox<ItemDisplay> grabbedReference, ItemStack itemStack, Func<Transform?>? parentSupplier = null) {
		ItemDisplay grabbed = CreateGrabbedDisplay(itemStack, parentSupplier);
		grabbedReference.value = grabbed;
	}

	[InterpretationFunctionCandidate]
	public void TransferGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
		if (!PopupOpen) {
			return;
		}
		PlayerController player = GameManager.instance.Player;
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
		if (slot.ContainedItem != grabbedItem.value!.ItemStack.item || grabbedItem.value.ItemStack.IsFull() || slot.ItemStack.IsFull()) {
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
		slot.DetachItemDisplay();
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
		slot.DetachItemDisplay();
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