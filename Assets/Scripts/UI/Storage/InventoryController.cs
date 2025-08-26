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
	private IItemSlot? dragSplitSource;
	private int startDragSplitStack;
	private Dictionary<IItemSlot, int> quantitySnapshots = new();

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
		SlotClickInterpretation interpretation = InterpretClick(slot, eventData);
		if (interpretation == null) {
			return;
		}
		InputHandler.RequestAction(new(interpretation.context, interpretation.priority, () => {
			ItemDisplay? grabbedReference = GrabbedItem;
			InvocationHelper.If(slot.ValidClickAction(GrabbedItem), () => {
				interpretation.interpretationFunction.Invoke(slot, ref grabbedReference);
				if (GrabbedItem != null) {
					this.StartDrag(slot);
				}
			});
			this.GrabbedItem = grabbedReference;
		}, null));
		InputHandler.BlockContext("ItemUse", () => !GameManager.instance.Player.InputHandler.LeftHold);
	}

	public void OnPointerUp(IItemSlot slot, PointerEventData eventData) { 
		InvocationHelper.If(draggingSlots, this.EndDrag);
	}

	public void OnPointerEnter(IItemSlot slot, PointerEventData eventData) {
		InvocationHelper.If(draggingSlots, () => this.OnSlotDrag(slot, eventData));
	}

	public SlotClickInterpretation InterpretClick(IItemSlot clickedSlot, PointerEventData eventData) {
		bool shift = Keyboard.current.shiftKey.isPressed;
		bool ctrl = Keyboard.current.ctrlKey.isPressed;
		bool alt = Keyboard.current.altKey.isPressed;

		if (eventData.button == PointerEventData.InputButton.Left) {
			if (!shift && !ctrl && !alt) {
				return new SlotClickInterpretation("ItemTransferGrabbed", 10, TransferGrabbed);
			}
		} else if (eventData.button == PointerEventData.InputButton.Right) {
			if (!shift && !alt && !ctrl) {
				return new SlotClickInterpretation("ItemTransferGrabbed", 10, HalveStack);
			}
		}
		return null!;
	}

	public delegate void InterpretationFunction(IItemSlot clickedSlot, ref ItemDisplay? grabbedItem);

	public record SlotClickInterpretation {
		public string context;
		public int priority;
		public InterpretationFunction interpretationFunction;

		public SlotClickInterpretation(string context, int priority, InterpretationFunction interpretationFunction) {
			this.context = context;
			this.priority = priority;
			this.interpretationFunction = interpretationFunction;
		}
	}

	public void HalveStack(IItemSlot clickedSlot, ref ItemDisplay? grabbedItem) {
		if (grabbedItem != null && clickedSlot.HasItem) {
			ItemStack stackReference = clickedSlot.ItemStack!;
			this.AddToGrabbed(ref stackReference, ref grabbedItem, 1);
			clickedSlot.ItemStack!.Quantity = stackReference.Quantity;
			return;
		}
		if (clickedSlot.IsEmpty) {
			this.TransferGrabbed(clickedSlot, ref grabbedItem);
			return;
		}

		int amount = clickedSlot.ItemStack!.Quantity / 2;
		if (clickedSlot.ItemStack.Quantity % 2 == 1) {
			amount++;
		}
		clickedSlot.ItemStack.Quantity -= amount;
		CreateGrabbedDisplay(ref grabbedItem, new ItemStack(clickedSlot.ItemStack.Item, amount));
	}

	public void AddToGrabbed(ref ItemStack from, ref ItemDisplay? grabbedItem, int amount) {
		if (grabbedItem == null) {
			CreateGrabbedDisplay(ref grabbedItem, new ItemStack(from.Item, 0));
		}
		if (from.Quantity < amount) {
			grabbedItem!.ItemStack.Quantity += from.Quantity;
			from.Quantity = 0;
		} else {
			from.Quantity -= amount;
			grabbedItem!.ItemStack.Quantity += amount;
		}
	}

	public ItemDisplay CreateGrabbedDisplay(ItemStack itemStack, Func<Transform?>? parentSupplier = null) {
		ItemDisplay grabbed = ItemDisplay.Create(itemStack, parentSupplier ?? (() => this.transform));
		grabbed.EnableGrab();
		return grabbed;
	}

	public void CreateGrabbedDisplay(ref ItemDisplay? grabbedReference, ItemStack itemStack, Func<Transform?>? parentSupplier = null) {
		ItemDisplay grabbed = CreateGrabbedDisplay(itemStack, parentSupplier);
		grabbedReference = grabbed;
	}

	public void TransferGrabbed(IItemSlot slot, ref ItemDisplay? grabbedItem) {
		if (!PopupOpen) {
			return;
		}
		PlayerController player = GameManager.instance.Player;
		void SetRightClickAvailable(bool enabled) {
			Action<ItemUseTrigger[]> action = enabled ? player.ItemUsageHandler.Enable : player.ItemUsageHandler.Disable;
			action.Invoke(new ItemUseTrigger[] { ItemUseTrigger.RightClick, ItemUseTrigger.RightHold });
		}

		// Grab item
		if (grabbedItem == null && slot.HasItem) {
			SetRightClickAvailable(false);
			player.SetMainHandItem(slot.ItemStack!);
			grabbedItem = slot.ItemDisplay;
			slot.DetachItemDisplay();
			return;
		}

		// Release item
		if (grabbedItem != null && !slot.HasItem) {
			SetRightClickAvailable(true);
			(slot as IItemSlot).AttachItemDisplay(grabbedItem);
			player.SetMainHandItem(Hotbar.ActiveSlot.ItemStack);
			grabbedItem = null;
			return;
		}

		// Swap items
		if (slot.ItemStack?.Item != grabbedItem!.ItemStack.Item || grabbedItem.ItemStack.HasMaxStack() || slot.ItemStack.HasMaxStack()) {
			ItemDisplay previousGrabbed = grabbedItem;
			grabbedItem = slot.ItemDisplay;
			slot.DetachItemDisplay();
			slot.AttachItemDisplay(previousGrabbed);
			player.SetMainHandItem(grabbedItem.ItemStack);
		} else {    // Merge/flow items
			int space = slot.ItemStack.Item.maxStackSize - slot.ItemStack.Quantity;
			int transfer = Math.Min(space, grabbedItem.ItemStack.Quantity);
			slot.ItemStack.Quantity += transfer;
			grabbedItem.ItemStack.Quantity -= transfer;
			if (grabbedItem.ItemStack.Quantity <= 0) {
				SetRightClickAvailable(true);
				grabbedItem.Destroy();
				grabbedItem = null;
			}
		}
	}

	public void StartDrag(IItemSlot dragSplitSource) { 
		draggingSlots = true;
		this.dragSplitSource = dragSplitSource;
		this.startDragSplitStack = dragSplitSource.ItemStack!.Quantity;
		quantitySnapshots.Clear();
		foreach (var slot in slots.Where(s => s != this.dragSplitSource && s.ContainedItem == dragSplitSource.ContainedItem)) {
			quantitySnapshots[slot] = slot.ItemStack!.Quantity;
		}
		draggedSlots.Add(dragSplitSource);
	}

	public void OnSlotDrag(IItemSlot draggedSlot, PointerEventData eventData) {
		if (!draggingSlots) {
			return;
		}
		if (draggedSlots.Contains(draggedSlot) || (draggedSlot.HasItem && draggedSlot.ContainedItem != dragSplitSource!.ContainedItem)) {
			return;
		}
		// Clone to preview distribution
		List<IItemSlot> cloned = new(draggedSlots) { draggedSlot };

		int toSplit = startDragSplitStack;
		int splitAmount = toSplit / (cloned.Count);
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
				slot.CreateDisplay(new ItemStack(dragSplitSource!.ItemStack!.Item, amount));
			}
			if (quantitySnapshots.TryGetValue(slot, out var quantity) && slot != this.dragSplitSource) {
				slot.ItemStack!.Quantity = quantity + amount;
			} else {
				slot.ItemStack!.Quantity = amount;
			}
		}
	}

	public void EndDrag() {
		if (draggedSlots.Count == 0) {
			draggingSlots = false;
			this.dragSplitSource = null;
			return;
		}

		draggingSlots = false;
		draggedSlots.Clear();
		this.dragSplitSource = null;
	}
}