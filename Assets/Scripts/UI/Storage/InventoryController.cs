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

public class InventoryController : MonoBehaviour, IContainer, IDependencyInitializable<InventoryController, PlayerController> {
	[SerializeField] private HotbarController hotbar;
	public HotbarController Hotbar => hotbar;

	[SerializeField] private GameObject popup;
	[SerializeField] private GameObject armorSlots;
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

	public InventorySlot[,] popupSlots;
	public IItemSlot this[int row, int col] => popupSlots[row, col];

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
		return this;
	}

	public void SetupGrid() {
		InventorySlot[] flatPopupSlots = popup.GetComponentsInChildren<InventorySlot>(true);
		popupSlots = ArrayHelper.CompressTo2D(flatPopupSlots, rows, columns);
	}

	private void Start() {
		CreateItemDisplay(new ItemStack(Items.grassBlock, 100), hotbar[2]);
		hotbar.SetActiveSlot(0);
	}

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
				CreateItemDisplay(itemStack, emptySlot);
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
					CreateItemDisplay(new ItemStack(itemStack.Item, toAdd), inventory[i]);
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
	
	public ItemDisplay CreateItemDisplay(ItemStack itemStack, InventorySlot slot) {
		GameObject obj = Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("itemDisplayPrefab"), slot.transform);
		ItemDisplay display = obj.GetComponent<ItemDisplay>();
		UnityEngine.Debug.Assert(display != null, $"ItemDisplay instance not found in item display prefab");
		display.ItemStack = itemStack;
		if (itemStack.Item.IsStackable) {
			itemStack.InitializeStackText(display);
		}
		display.Tooltip = itemStack.Item.GetTooltip();
		return display;
	}

	public ItemDisplay CreateItemDisplay(Item item, int quantity, InventorySlot slot) {
		ItemStack itemStack = new(item, quantity);
		return CreateItemDisplay(itemStack, slot);
	}

	public void DestroyItemDisplay(ItemDisplay display) {
		if (display.ItemStack == player.MainHandStack) {
			player.SetMainHandItem(null);
		}
		GameObject.Destroy(display.gameObject);
	}

	public void EquipHotbarItem(InventorySlot slot) {
		ItemDisplay itemDisplay = slot.ItemDisplay;
		player.SetMainHandItem(itemDisplay?.ItemStack);
	}
}