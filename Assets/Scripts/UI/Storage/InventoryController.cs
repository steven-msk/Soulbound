using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour, IContainer<InventorySlot>, IDependencyInitializable<InventoryController, PlayerController>, ISerializable<SerializedInventory> {
	private static readonly Logger logger = Logger.CreateInstance();
	[SerializeField] private HotbarController hotbar;
	public HotbarController Hotbar => hotbar;

	[SerializeField] private GameObject popup;
	[SerializeField] private GameObject armorSlots;
	private static Dictionary<int, ArmorSlot> armorSlotsByIndex = new();
	[SerializeField] private bool popupOpen = false;

	public bool PopupOpen => popupOpen;
	public ItemDisplay GrabbedItem { get; set; }

	[SerializeField] private AbstractTooltip activeTooltip;
	public AbstractTooltip ActiveTooltip { set => activeTooltip = value; }

	public InventorySlot[] MainPlayerSlots { get; private set; }

	[SerializeField] private int rows;
	public int Rows => rows;

	[SerializeField] private int columns;
	public int Columns => columns;

	private static Dictionary<int, InventorySlot> popupSlotsByIndex = new();
	public InventorySlot[,] popupSlots;
	public InventorySlot this[int row, int col] => popupSlots[row, col];

	private PlayerController player;


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

	//private void Start() {
	//	ItemDisplay.Create(new ItemStack(Items.grassBlock, 100), hotbar[2]);
	//}

    public void ToggleInventory() {
		popupOpen = !popupOpen;
		popup.SetActive(popupOpen);
		armorSlots.SetActive(popupOpen);
		LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
		activeTooltip?.Hide();
		hotbar.OnInventoryPopup();
	}

	public void DropItemFromInventory() {
		if (activeTooltip != null) {
			ItemDisplay itemDisplay = activeTooltip.DisplayParent?.GetComponent<ItemDisplay>();
			itemDisplay?.ItemStack.Drop(player.center, player.itemDropForce, true);
			Destroy(itemDisplay?.gameObject);
			activeTooltip.Hide();
			activeTooltip = null;
		} else {
			ItemDisplay item = hotbar.ActiveSlot.ItemDisplay;
			item?.ItemStack.Drop(player.center, player.itemDropForce, true);
			Destroy(item?.gameObject);
		}
	}

	// POTENTIAL: OnDrop callback in Item

	public void DropGrabbedItem() {
		GrabbedItem?.ItemStack.Drop(player.center, player.itemDropForce, true);
		Destroy(GrabbedItem?.gameObject);
		if (GrabbedItem?.ItemStack == player.MainHandStack) {
			player.SetMainHandItem(null);
		}
		GrabbedItem = null;
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
			InventorySlot[] inventory = MainPlayerSlots;
			for (int i = 0; i < inventory.Length; i++){
				if (!inventory[i].HasItem) {
					int toAdd = Mathf.Min(remaining, itemStack.Item.maxStackSize);
					ItemDisplay.Create(new ItemStack(itemStack.Item, toAdd), inventory[i]);
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
		player.SetMainHandItem(itemDisplay?.ItemStack);
	}

	public InventorySlot GetSlotByIndex(int index) => popupSlotsByIndex[index];
}