using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour, IContainer {
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

	private bool setup = false;

	// FEATUREIMPL: item grabbing controls - this might require a general implementation in IContainer

	private void Awake() {
		player = GameManager.instance.Player;
		LogUtil.LogAwake(this);
	}

#nullable enable
	public void SetupGrid(Action? callback) {
		InventorySlot[] flatPopupSlots = popup.GetComponentsInChildren<InventorySlot>(true);
		popupSlots = ArrayHelper.CompressTo2D(flatPopupSlots, rows, columns);
		callback?.Invoke();

		List<InventorySlot> mainPlayerSlots = hotbar.Slots.ToList();
		mainPlayerSlots.AddRange(popupSlots);
		MainPlayerSlots = mainPlayerSlots.ToArray();
		setup = true;
	}
#nullable disable

	private void Start() {
		IEnumerator Prototype_setupDisplays() {
			yield return new WaitUntil(() => setup);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("weaponItem_test"), 2), hotbar[0]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("armoritem_test"), 2), hotbar[1]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("armoritem_test"), 2), popupSlots[0, 0]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("consumableStatItem_test"), 100), hotbar[3]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("consumableStatItem_test"), 21_489), hotbar[6]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("consumableStatItem_test"), 1), hotbar[7]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("consumableStatItem_test"), 1), hotbar[8]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("consumableStatItem_test"), 1), (InventorySlot)this[2, 8]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("longTooltipItem"), 1), hotbar[5]);
			CreateItemDisplay(new ItemStack(Items.grassBlock, 100), hotbar[2]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("wood_block_item"), 100), (InventorySlot)this[0, 1]);
			//CreateItemDisplay(new ItemStack(AssetRegistry.Get<Item>("tree_sapling_item"), 10), (InventorySlot)this[0, 2]);
			Debug.Log("<color=green>[INVENTORY]</color> Player inventory loaded");				// might factor out in LogUtil
			// POTENTIAL: LogUtil modularity - implement different color coded logging sections marked between [ ]
			hotbar.SetActiveSlot(0);
		}
		StartCoroutine(Prototype_setupDisplays());
	}

    public void ToggleInventory(InputAction.CallbackContext actionContext) {
		popupOpen = !popupOpen;
		popup.SetActive(popupOpen);
		armorSlots.SetActive(popupOpen);
		LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
		activeTooltip?.Hide();
		hotbar.OnInventoryPopup();
	}

	public void DropItemFromInventory(InputAction.CallbackContext actionContext) {
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

	public void DropGrabbedItem(InputAction.CallbackContext actionContext) {
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
		GameObject obj = Instantiate(AssetRegistry.Get<GameObject>("itemDisplayPrefab"), slot.transform);
		ItemDisplay display = obj.GetComponent<ItemDisplay>();
		Debug.Assert(display != null, $"ItemDisplay instance not found in item display prefab");
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