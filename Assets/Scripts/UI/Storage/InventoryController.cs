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

public class InventoryController : MonoBehaviour {

	// PLANNED: IContainer interface for shareable item container behavior
	// PLANNED REFACTOR: adapt InventoryController to have shared container behavior

	[SerializeField] private HotbarController hotbar;
	public HotbarController Hotbar => hotbar;

	[SerializeField] private GameObject popup;
	[SerializeField] private bool popupOpen = false;

	public bool PopupOpen => popupOpen;
	[SerializeField] private ItemDisplay grabbedItem = null;
	public ItemDisplay PickupItem => grabbedItem;
	public readonly Dictionary<int, InventorySlot> popupSlots = new();
	[SerializeField] private AbstractTooltip activeTooltip;
	public AbstractTooltip ActiveTooltip { set => activeTooltip = value; }

	public InventorySlot[] AllSlots { get; private set; }

	private PlayerController player;

	// FIXME: hotbar not prioritised when picking up items

	private void Awake() {
		player = GameManager.GetPlayerInstance();

		InventorySlot[] slots = popup.GetComponentsInChildren<InventorySlot>(true);
		slots = slots.OrderBy(slot => Regex.Match(slot.name, @"Slot (\d+)")?.Groups[1].Value).ToArray();
		for (int i = 0; i < slots.Length; i++) {
			popupSlots[i + 1] = slots[i];
		}
		IEnumerator HotbarLoadCallback() {
			yield return new WaitUntil(() => hotbar.slots.Count == 9);
			slots.ToList().AddRange(hotbar.slots.Values);
			AllSlots = slots.ToArray();
		}
		StartCoroutine(HotbarLoadCallback());
	}

	private void Start() {
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("weaponItem_test"), 2), hotbar.slots[1]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("consumableStatItem_test"), 100), hotbar.slots[3]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("consumableStatItem_test"), 21_489), hotbar.slots[6]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("consumableStatItem_test"), 1), hotbar.slots[7]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("equipableItemTest"), 1), hotbar.slots[2]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("equipableItemTest"), 1), hotbar.slots[4]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("longTooltipItem"), 1), hotbar.slots[5]);
	}

	public void ToggleInventory(InputAction.CallbackContext actionContext) {
		popupOpen = !popupOpen;
		popup.SetActive(popupOpen);
		LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
		if (activeTooltip != null && activeTooltip.IsDisplayed) {
			activeTooltip?.Hide();
		}
		hotbar.OnInventoryPopup(); 
	}

	public void DropItemFromInventory(InputAction.CallbackContext actionContext) {
		if (activeTooltip != null) {
			ItemDisplay itemDisplay = activeTooltip.DisplayParent?.GetComponent<ItemDisplay>();
			itemDisplay?.ItemStack.Drop(true);
			Destroy(itemDisplay?.gameObject);
			activeTooltip.Hide();
			activeTooltip = null;
		} else {
			ItemDisplay item = hotbar.ActiveSlot.ItemDisplay;
			item?.ItemStack.Drop(true);
			Destroy(item?.gameObject);
		}
	}

	public void DropGrabbedItem(InputAction.CallbackContext actionContext) {
		if (grabbedItem != null) {
			grabbedItem.ItemStack.Drop(true);
			Destroy(grabbedItem.gameObject);
			if (grabbedItem.ItemStack == player.MainHandStack) {
				player.SetMainHandItem(null);
			}
			grabbedItem = null;
		}
	}

	public bool GrabItem(ItemStack itemStack) {
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
				if (stackInSlot.Quantity < stackInSlot.Item.MaxStackSize) {
					int availableSpace = stackInSlot.Item.MaxStackSize - stackInSlot.Quantity;
					int toAdd = Math.Min(remaining, availableSpace);
					stackInSlot.Quantity += toAdd;
					remaining -= toAdd;
					if (remaining <= 0) {
						return true;
					}
				}
			}


			InventorySlot[] inventory = AllSlots;
			for (int i = 0; i < inventory.Length; i++){
				if (!inventory[i].HasItem) {
					int toAdd = Mathf.Min(remaining, itemStack.Item.MaxStackSize);
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

	[InputAction("ItemDrag", Priority = 10, BlocksContexts = new[] { "ItemUse" })]
	public void OnSlotClick(IItemSlot slot) {
		if (!popupOpen || (slot.IsEmpty && grabbedItem == null)) {
			return;
		}

		if (grabbedItem == null && slot.HasItem) {
			grabbedItem = slot.ItemDisplay;
			grabbedItem.EnableMoveMode();
			grabbedItem.transform.SetParent(gameObject.transform, true);
			player.ItemUsageHandler.Disable(ItemUseTrigger.RightClick, ItemUseTrigger.RightHold);
			player.InputHandler.inputActions.Player.RightClick.performed += DropGrabbedItem;
			player.SetMainHandItem(grabbedItem.ItemStack);
			return;
		}

		if (slot.IsEmpty) {
			grabbedItem.transform.SetParent(slot.GameObject.transform, true);
			grabbedItem.DisableMoveMode();
			grabbedItem = null;
			player.ItemUsageHandler.Enable(ItemUseTrigger.RightClick, ItemUseTrigger.RightHold);
			player.InputHandler.inputActions.Player.RightClick.performed -= DropGrabbedItem;
			player.SetMainHandItem(hotbar.ActiveSlot.ItemStack);
			return;
		}

		ItemDisplay itemDisplay = slot.ItemDisplay;
		ItemStack pickupStack = grabbedItem.ItemStack;
		ItemStack slotStack = itemDisplay.ItemStack;
		if (slotStack.Item != pickupStack.Item || slotStack.Quantity == slotStack.Item.MaxStackSize) {
			grabbedItem.transform.SetParent(slot.GameObject.transform, true);
			grabbedItem.DisableMoveMode();
			grabbedItem = itemDisplay;
			grabbedItem.EnableMoveMode();
			grabbedItem.transform.SetParent(gameObject.transform, true);
			GameManager.GetPlayerInstance().SetMainHandItem(grabbedItem.ItemStack);
		} else {
			int space = slotStack.Item.MaxStackSize - slotStack.Quantity;
			int transfer = Math.Min(space, pickupStack.Quantity);
			slotStack.Quantity += transfer;
			pickupStack.Quantity -= transfer;
			if (pickupStack.Quantity <= 0) {
				Destroy(grabbedItem.gameObject);
				grabbedItem = null;
				player.ItemUsageHandler.Enable(ItemUseTrigger.RightClick, ItemUseTrigger.RightHold);
				player.InputHandler.inputActions.Player.RightClick.performed -= DropGrabbedItem;
			}
		}
	}

	[InputAction("ItemDrag", Priority = 10, BlocksContexts = new[] { "ItemUse" })]
	public void OnEquipmentSlotClicked(EquipmentSlot slot) {
		if (grabbedItem?.ItemStack.Item is not IEquipable && grabbedItem != null) {
			return;
		}
		bool justEquipped = false;
		if (grabbedItem?.ItemStack.Item is IEquipable equipable && !slot.HasItem) {
			equipable.OnEquip(slot);
			justEquipped = true;
		} else if (slot.HasItem) {
			((IEquipable)slot.ItemStack.Item).OnUnequipped(slot.ItemStack.Item);
		}
		
		this.OnSlotClick(slot);
		if (slot.HasItem && !justEquipped) {
			((IEquipable)slot.ItemStack.Item).OnEquip(slot);
		}
	}

	[CanBeNull] public InventorySlot GetFirstEmptySlot() => AllSlots.First(slot => slot.IsEmpty);

	[CanBeNull] public InventorySlot[] GetOccupiedSlots() => AllSlots.Where(slot => slot.HasItem).ToArray();

	[CanBeNull] public InventorySlot[] GetOccupiedSlots(Item item) => GetOccupiedSlots().Where(slot => slot.ItemStack.Item.Equals(item)).ToArray();

	[CanBeNull] public InventorySlot[] GetEmptySlots() => AllSlots.Where(slot => !slot.HasItem).ToArray();
	
	public ItemDisplay CreateItemDisplay(ItemStack itemStack, InventorySlot slot) {
		GameObject obj = Instantiate(Registry.Get<GameObject>("itemDisplayPrefab"), slot.transform);
		ItemDisplay display = obj.GetComponent<ItemDisplay>();
		Debug.Assert(display != null, $"ItemDisplay instance not found in item display prefab");
		display.ItemStack = itemStack;
		itemStack.Initialize(display);
		display.Tooltip = itemStack.Item.GetTooltip();
		return display;
	}

	public ItemDisplay CreateItemDisplay(Item item, int quantity, InventorySlot slot) {
		ItemStack itemStack = new(item, quantity);
		return CreateItemDisplay(itemStack, slot);
	}

	public void DestroyItemDisplay(ItemDisplay display) {
		PlayerController player = GameManager.GetPlayerInstance();
		if (display.ItemStack == player.MainHandStack) {
			player.SetMainHandItem(null);
		}
		GameObject.Destroy(display.gameObject);
	}

	public void EquipHotbarItem(InventorySlot slot) {
		ItemDisplay itemDisplay = slot.ItemDisplay;
		PlayerController player = GameManager.GetPlayerInstance();
		player.SetMainHandItem(itemDisplay?.ItemStack);
	}
}