using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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
		if (!itemStack.Item.IsStackable) {
			InventorySlot emptySlot = GetFirstEmptySlot();
			if (emptySlot != null) {
				ItemDisplay.Create(itemStack, emptySlot);
				return true;
			}
			return false;
		} else {
			int remaining = itemStack.Quantity;

			// Search for stack with the same item
			foreach (var stackSlot in GetOccupiedSlots(itemStack.Item)) {
				ItemStack stackInSlot = stackSlot.ItemStack;
				if (stackInSlot.Quantity < stackInSlot.Item.maxStackSize) {
					int availableSpace = stackInSlot.Item.maxStackSize - stackInSlot.Quantity;
					int toAdd = Math.Min(remaining, availableSpace);
					stackInSlot.Quantity += toAdd;
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
					int toAdd = Mathf.Min(remaining, itemStack.Item.maxStackSize);
					ItemDisplay.Create(new ItemStack(itemStack.Item, toAdd), availableSlots[i]);
					remaining -= toAdd;
				}
				if (remaining <= 0) {
					return true;
				}
			}
			return remaining <= 0;
		}
	}

	[CanBeNull] public InventorySlot GetFirstEmptySlot() => MainPlayerSlots.First(slot => slot.IsEmpty);

	[CanBeNull] public InventorySlot[] GetOccupiedSlots() => MainPlayerSlots.Where(slot => slot.HasItem).ToArray();

	[CanBeNull] public InventorySlot[] GetOccupiedSlots(Item item) => GetOccupiedSlots().Where(slot => slot.ItemStack.Item.Equals(item)).ToArray();

	[CanBeNull] public InventorySlot[] GetEmptySlots() => MainPlayerSlots.Where(slot => !slot.HasItem).ToArray();
	
	public void EquipHotbarItem(InventorySlot slot) {
		ItemDisplay itemDisplay = slot.ItemDisplay;
		player.SetMainHandItem(itemDisplay?.ItemStack!);
	}

	public InventorySlot GetSlotByIndex(int index) => popupSlotsByIndex[index];
	IItemSlot IItemContainer.GetSlotByIndex(int index) => popupSlotsByIndex[index];

	public void OnPointerDown(IItemSlot slot, PointerEventData eventData) {
		this.dragButton = eventData.button;
		InterpretationFunction? interpretationFunction = InterpretClick(slot, eventData);
		if (interpretationFunction == null) {
			return;
		}
		InputHandler.RequestAction(new("ItemSlotAction", 10, () => {
			RefBox<ItemDisplay> grabbedReference = new(this.GrabbedItem);
			interpretationFunction.Invoke(slot, grabbedReference);
			if (GrabbedItem != null) {
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
		ItemDisplay? grabbedReference = GrabbedItem;
		InvocationHelper.If(draggingSlots, () => this.OnSlotDrag(slot, ref grabbedReference));
		GrabbedItem = grabbedReference;
	}

	public InterpretationFunction? InterpretClick(IItemSlot clickedSlot, PointerEventData eventData) {
		bool shift = Keyboard.current.shiftKey.isPressed;
		bool ctrl = Keyboard.current.ctrlKey.isPressed;
		bool alt = Keyboard.current.altKey.isPressed;

		if (eventData.button == PointerEventData.InputButton.Left) {
			if (!shift && !ctrl && !alt) {
				return TransferGrabbed;
			}
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			if (!shift && !alt && !ctrl) {
				if (GrabbedItem != null && !clickedSlot.HasItem) {
					return TransferSingleToSlot;
				}
				if (GrabbedItem != null) {
					return HalveStackFromSlot;
				}
				return HalveStackFromSlot;
			}
		}
		return null!;
	}

	[InterpretationFunctionSubmodule]
	public void HalveStackFromSlot(IItemSlot clickedSlot, RefBox<ItemDisplay> grabbedItem) {
		if (clickedSlot.ItemStack == null || grabbedItem.value != null) {
			return;
		}
		int amount = clickedSlot.ItemStack.Quantity / 2;
		amount += clickedSlot.ItemStack.Quantity % 2;
		clickedSlot.ItemStack.Quantity -= amount;
		CreateGrabbedDisplay(grabbedItem, new ItemStack(clickedSlot.ItemStack.Item, amount));
	}

	[InterpretationFunctionSubmodule]
	public void TransferSingleToSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
		if (grabbedItem.value == null) {
			return;
		}
		slot.TryAddStack(1, grabbedItem.value.ItemStack.Item);
		grabbedItem.value.ItemStack.Quantity--;
	}

	[InterpretationFunctionSubmodule]
	public void TransferSingleToGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
		if (slot.ItemStack == null) {
			return;
		}
		this.AddToGrabbed(slot, grabbedItem, 1);
		slot.ItemStack.Quantity--;
	}

	[InterpretationFunctionSubmodule]
	public void AddToGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem, int amount) {
		if (slot.ItemStack == null) {
			return;
		}
		if (grabbedItem.value == null) {
			CreateGrabbedDisplay(grabbedItem, new ItemStack(slot.ItemStack.Item, 0));
		}
		int transfer = Mathf.Min(amount, slot.ItemStack.Quantity);
		slot.ItemStack.Quantity -= transfer;
		grabbedItem.value!.ItemStack.Quantity += transfer;
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

	[InterpretationFunctionSubmodule]
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
		if (grabbedItem.value == null && slot.HasItem) {
			SetRightClickAvailable(false);
			this.GrabItemFromSlot(slot, grabbedItem);
			return;
		}

		// Release if slot is empty, grabbed has item
		if (grabbedItem.value != null && !slot.HasItem) {
			SetRightClickAvailable(true);
			this.ReleaseItemInSlot(slot, grabbedItem);
			return;
		}

		// Swap items
		if (slot.ItemStack?.Item != grabbedItem.value!.ItemStack.Item || grabbedItem.value.ItemStack.HasMaxStack() || slot.ItemStack.HasMaxStack()) {
			this.SwapItems(slot, grabbedItem);
		} else {    // Merge/flow items
			SetRightClickAvailable(MergeFromGrabbed(slot, grabbedItem));
		}
	}

	[InterpretationFunctionSubmodule]
	public void GrabItemFromSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
		if (slot.IsEmpty || grabbedItem.value != null) {
			return;
		}
		grabbedItem.value = slot.ItemDisplay;
		slot.DetachItemDisplay();
		player.SetMainHandItem(grabbedItem.value.ItemStack);
	}

	[InterpretationFunctionSubmodule]
	public void ReleaseItemInSlot(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
		if (slot.HasItem || grabbedItem.value == null) {
			return;
		}
		(slot as IItemSlot).AttachItemDisplay(grabbedItem.value);
		player.SetMainHandItem(Hotbar.ActiveSlot.ItemStack);
		grabbedItem.value = null;
	}

	[InterpretationFunctionSubmodule]
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

	[InterpretationFunctionSubmodule("Returns true if the grabbed stack fit in the slot stack")]
	public bool MergeFromGrabbed(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
		int space = slot.ItemStack.Item.maxStackSize - slot.ItemStack.Quantity;
		int transfer = Math.Min(space, grabbedItem.value.ItemStack.Quantity);
		slot.ItemStack.Quantity += transfer;
		grabbedItem.value.ItemStack.Quantity -= transfer;
		if (grabbedItem.value.ItemStack.Quantity <= 0) {
			grabbedItem.value.Destroy();
			grabbedItem.value = null;
			return true;
		}
		return false;
	}

	public void StartDrag(IItemSlot dragOrigin) { 
		draggingSlots = true;
		this.dragOrigin = dragOrigin;
		this.startDragStack = dragOrigin.ItemStack!.Quantity;
		quantitySnapshots.Clear();
		foreach (var slot in slots.Where(s => s != this.dragOrigin && s.ContainedItem == dragOrigin.ContainedItem)) {
			quantitySnapshots[slot] = slot.ItemStack!.Quantity;
		}
		Debug.Log("starting drag: " + dragButton);
		draggedSlots.Add(dragOrigin);
	}

	public void OnSlotDrag(IItemSlot draggedSlot, ref ItemDisplay? grabbedItem) {
		if (!draggingSlots) {
			return;
		}

		if (dragButton == PointerEventData.InputButton.Left) {
			if (draggedSlots.Contains(draggedSlot) || (draggedSlot.HasItem && draggedSlot.ContainedItem != dragOrigin!.ContainedItem)) {
				return;
			}

			// Clone to preview distribution
			List<IItemSlot> preview = new(draggedSlots) { draggedSlot };

			int toSplit = startDragStack;
			int splitAmount = toSplit / (preview.Count);
			if (splitAmount <= 0) {
				return;
			}
			// Commit the slot to dragged list
			draggedSlots.Add(draggedSlot);
			int remainder = toSplit % (draggedSlots.Count);

			for (int i = 0; i < draggedSlots.Count; i++) {
				var slot = draggedSlots[i];
				int amount = splitAmount + (i < remainder ? 1 : 0);

				if (slot.ItemStack == null) {
					slot.CreateDisplay(new ItemStack(dragOrigin!.ItemStack!.Item, amount));
				}
				if (quantitySnapshots.TryGetValue(slot, out var quantity) && slot != this.dragOrigin) {
					slot.ItemStack!.Quantity = quantity + amount;
				} else {
					slot.ItemStack!.Quantity = amount;
				}
			}
		} else if (dragButton == PointerEventData.InputButton.Right && grabbedItem != null && grabbedItem.ItemStack.Quantity > 0) {
			draggedSlot.TryAddStack(1, grabbedItem.ItemStack.Item);
			grabbedItem.ItemStack.Quantity--;
		}
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
}