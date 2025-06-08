using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {

	[SerializeField] private GameObject itemDropTrigger;
	[SerializeField] private HotbarController hotbar;
	public HotbarController Hotbar => hotbar;

	[SerializeField] private GameObject popup;
	[SerializeField] private bool popupOpen = false;

	public bool PopupOpen => popupOpen;
	[SerializeField] private ItemDisplay pickupItem = null;
	public readonly Dictionary<int, StorageSlot> popupSlots = new();
	[SerializeField] private AbstractTooltip activeTooltip;
	public AbstractTooltip ActiveTooltip { set => activeTooltip = value; }
	public StorageSlot[] AllSlots {		// FIXME not optimal
		get {
			List<StorageSlot> slots = new();
			slots.AddRange(hotbar.slots.Values);
			slots.AddRange(popupSlots.Values);
			return slots.ToArray();
		}
	}

	private void Awake() {
		StorageSlot[] slots = popup.GetComponentsInChildren<StorageSlot>(true);
		slots = slots.OrderBy(slot => Regex.Match(slot.name, @"Slot (\d+)")?.Groups[1].Value).ToArray();
		for (int i = 0; i < slots.Length; i++) {
			popupSlots[i + 1] = slots[i];
		}
	}

	private void Start() {
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("sword"), 1, CompoundTooltip.OfCustom(CompoundTooltipLayout.SpacingOnly(15), Tooltip.Title("Sword"), Tooltip.Stats(new Dictionary<string, object> {
			{ "<color=red>Damage</color>", 8 },
			{ "Crit chance", "5%" }
		}), Tooltip.Lore("A mighty sword.")).Data), hotbar.slots[1]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("gem"), 100, Tooltip.Plain("Blue Gem").Data), hotbar.slots[2]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("gem_sword"), 1, Tooltip.Compound(Tooltip.Title("Gem Sword"), Tooltip.Stats(new Dictionary<string, object> {
			{ "Damage", 150 },
			{ "Crit chance", "12%" },
			{ $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(new Color(1f, 0.99f, 0.85f))}>The Holy Revolution</color> - On every 5th hit there's a 5% chance to spawn an angel that fights for you.", "Blessing:" }
		}), Tooltip.Lore("Angels cherish this weapon - use it carefully.")).Data), hotbar.slots[3]);
		CreateItemDisplay(new ItemStack(Registry.Get<Item>("consumableItem_test"), 1, Tooltip.Plain("Item").Data), hotbar.slots[9]);
		//CreateItemDisplay(new ItemStack(Registry.Get<Item>("consumableItem_test"), 50, Tooltip.Plain("Blue Gem").Data), hotbar.slots[8]);
		CreateItemDisplay(Registry.Get<Item>("heavens_judgement"), 1, hotbar.slots[5], CompoundTooltip.Of(new TooltipData[] {
			new(new TooltipSectionLayout(TooltipSection.Title) {
				textColor = Color.cyan
			}, "Heaven's Judgement"),

			new(new TooltipSectionLayout(TooltipSection.Stats) {
				textColor = Color.white
			}, "154 Physical Damage\n" +
				"38 Soul Damage\n" +
				"1.25 Attack Speed\n" +
				"17% Crit Chance\n" +
				"2.1x Crit Multiplier\n" +
				"+2 Dash Velocity\n" +
				"-0.5s Dash Cooldown"),

			new(new TooltipSectionLayout(TooltipSection.Affixes) {
				textColor = new Color(1f, 0.9f, 0.5f)
			}, "Blessing: Radiant Edge — Attacks burn enemies with divine flame, dealing 24 Soul Damage over 3s.\n" +
				"Blessing: Light's Oath — On dash, gain 10% movement speed for 2s. Refreshes on kill."),
			// TODO: implement affix and ritual tooltip display - rework TooltipSection instance relation

			new(new TooltipSectionLayout(TooltipSection.Lore), "Forged at the summit of Solspire by the last remaining Starforger, this blade was said to sever the bonds of darkness itself.")
		}).Data);
	}

	[EventContextHandler("InventoryOpen")]
	public void ToggleInventory(InputAction.CallbackContext actionContext) {
		try { EventPriorityManager.RequestControl(new PrioritizedEvent("InventoryOpen", 10, "ItemUse")); } catch (PriorityRequestException) { }
		popupOpen = !popupOpen;
		if (!popupOpen) {
			EventPriorityManager.Revoke("InventoryOpen");
		}
		popup.SetActive(popupOpen);
		LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
		if (activeTooltip != null && activeTooltip.IsDisplayed) {
			activeTooltip?.Hide();
		}
		hotbar.SetEditMode(popupOpen);
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

	public bool PickupItem(ItemStack itemStack) {
		if (!itemStack.Item.IsStackable) {
			StorageSlot emptySlot = GetFirstEmptySlot();
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


			StorageSlot[] inventory = AllSlots;
			for (int i = 0; i < inventory.Length; i++){
				if (!inventory[i].HasItem) {
					int toAdd = Mathf.Min(remaining, itemStack.Item.MaxStackSize);
					CreateItemDisplay(new ItemStack(itemStack.Item, toAdd, itemStack.TooltipSerializer), inventory[i]);
					remaining -= toAdd;
				}
				if (remaining <= 0) {
					return true;
				}
			}
			return remaining <= 0;
		}
	}

	[RequiresEventContext("InventoryOpen", 100)]
	[EventContextHandler("ItemDrag")]
	public void OnSlotClick(StorageSlot slot) {
		if (!popupOpen) {
			return;
		}
		static IEnumerator RevokeInventoryCapabilities() {
			yield return null;
			EventPriorityManager.Revoke("ItemDrag");
		}

		if (pickupItem == null && slot.HasItem) {
			pickupItem = slot.ItemDisplay;
			pickupItem.EnableMoveMode();
			pickupItem.transform.SetParent(gameObject.transform, true);
			itemDropTrigger.SetActive(true);
			EventPriorityManager.RequestControl(new PrioritizedEvent("ItemDrag", 100));
			return;
		}

		ItemDisplay itemDisplay = slot.GetComponentInChildren<ItemDisplay>();
		if (itemDisplay == null) {
			pickupItem.transform.SetParent(slot.transform, true);
			pickupItem.DisableMoveMode();
			pickupItem = null;
			itemDropTrigger.SetActive(false);
			StartCoroutine(RevokeInventoryCapabilities());
			return;
		}

		ItemStack pickupStack = pickupItem.ItemStack;
		ItemStack slotStack = itemDisplay.ItemStack;
		if (slotStack.Item != pickupStack.Item || slotStack.Quantity == slotStack.Item.MaxStackSize) {
			pickupItem.transform.SetParent(slot.transform, true);
			pickupItem.DisableMoveMode();
			pickupItem = itemDisplay;
			pickupItem.EnableMoveMode();
			pickupItem.transform.SetParent(gameObject.transform, true);
		} else {
			int space = slotStack.Item.MaxStackSize - slotStack.Quantity;
			int transfer = Math.Min(space, pickupStack.Quantity);
			slotStack.Quantity += transfer;
			pickupStack.Quantity -= transfer;
			if (pickupStack.Quantity <= 0) {
				Destroy(pickupItem.gameObject);
				pickupItem = null;
				StartCoroutine(RevokeInventoryCapabilities());
			}
		}
	}

	[CanBeNull] public StorageSlot GetFirstEmptySlot() {
		foreach (var slot in AllSlots) {
			if (slot.IsEmpty) {
				return slot;
			}
		}
		return null;
	}

	[CanBeNull] public StorageSlot[] GetOccupiedSlots() => AllSlots.Where(slot => slot.HasItem).ToArray();

	[CanBeNull] public StorageSlot[] GetOccupiedSlots(Item item) => GetOccupiedSlots().Where(slot => slot.ItemStack.Item.Equals(item)).ToArray();

	[CanBeNull] public StorageSlot[] GetEmptySlots() => AllSlots.Where(slot => !slot.HasItem).ToArray();
	
	public ItemDisplay CreateItemDisplay(ItemStack itemStack, StorageSlot slot) {
		GameObject obj = Instantiate(Registry.Get<GameObject>("itemDisplayPrefab"), slot.transform);
		ItemDisplay display = obj.GetComponent<ItemDisplay>();
		Debug.Assert(display != null, $"ItemDisplay instance not found in item display prefab");
		display.ItemStack = itemStack;
		itemStack.Initialize(display);
		display.Tooltip = itemStack.TooltipSerializer.Generate();
		return display;
	}

	public ItemDisplay CreateItemDisplay(Item item, int quantity, StorageSlot slot, ITooltipSerializer tooltipSerializer) {
		ItemStack itemStack = new(item, quantity, tooltipSerializer);
		return CreateItemDisplay(itemStack, slot);
	}

	[EventContextHandler("ItemDrag")]
	public void OnDropItemTriggered() {
		pickupItem?.ItemStack.Drop(true);
		if (pickupItem != null) {
			Destroy(pickupItem.gameObject);
			pickupItem = null;
			EventPriorityManager.Revoke("ItemDrag");
		}
	}

	public void EquipHotbarItem(StorageSlot slot) {
		ItemDisplay itemDisplay = slot.ItemDisplay;
		PlayerController player = GameManager.GetPlayerInstance();
		player.EquipHotbarItem(itemDisplay?.ItemStack);
	}
}