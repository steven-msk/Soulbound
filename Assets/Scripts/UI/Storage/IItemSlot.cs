using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#nullable enable

public interface IItemSlot : IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, ISerializable<SerializedItemSlot> {
	public ItemDisplay ItemDisplay { get; }
	public IItemContainer2D container { get; }
	public int index { get; set; }
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;
	public ItemStack? ItemStack => ItemDisplay?.ItemStack;
	public Item? ContainedItem => ItemStack?.Item;
	public GameObject GameObject { get; }
	
	public void OnClick(ItemDisplay grabbedItem, InventoryController inventory);
	
	// PLANNED REFACTOR: attach and detach slot methods will cause problems later on with serializations
	// Attaching and detaching should only be made after the player released or grabbed an item from a slot.
	// This helps with serialization of items inside containers when the client crashes.
	// As of right now, if a client would crash while they have an item grabbed, there is
	// nowhere to place the item in the container since the data had already been detached.

	public virtual void AttachItemDisplay(ItemDisplay itemDisplay) {
		itemDisplay?.transform.SetParent(GameObject.transform, true);
		itemDisplay?.DisableGrab();
	}

	public virtual void DetachItemDisplay() {
		this.ItemDisplay.EnableGrab();
		this.ItemDisplay?.transform.SetParent(GameManager.instance.Player.Inventory.transform, true);
	}

	SerializedItemSlot ISerializable<SerializedItemSlot>.Serialize() => new(index, ItemStack);

	public void TrySetStack(int quantity, Item fallback) {
		CreateDisplayIfEmpty(new ItemStack(fallback, quantity));
		this.ItemStack!.Quantity = quantity;
	}

	public void TryAddStack(int add, Item fallback) {
		if (!CreateDisplayIfEmpty(new ItemStack(fallback, add))) {
			this.ItemStack!.Quantity += add;
		}
	}

	public void CreateDisplay(ItemStack itemStack) {
		this.AttachItemDisplay(ItemDisplay.Create(itemStack, () => GameObject.transform));
	}

	public bool CreateDisplayIfEmpty(ItemStack itemStack) {
		if (this.ItemStack == null) {
			CreateDisplay(itemStack);
			return true;
		}
		return false;
	}

	new public void OnPointerDown(PointerEventData eventData) {
		container.OnPointerDown(this, eventData);
	}
	new public void OnPointerUp(PointerEventData eventData) { 
		container.OnPointerUp(this, eventData);
	}
	new public void OnPointerEnter(PointerEventData eventData) { 
		container.OnPointerEnter(this, eventData);
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => this.OnPointerDown(eventData);
	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => this.OnPointerUp(eventData);
	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => this.OnPointerEnter(eventData);
}

#nullable enable
public static class ItemSlotUtility {

	// PLANNED REFACTOR: RequestClickAction() should take pointer event data for better functionality with item management in slots
	// This could require TransferItems() to accept multiple input triggers.
	// Since a lot of item management features are planned, TransferItems might become too full.
	// This is an anticipation of a separation of concerns regarding item transfer functionality.

	[Obsolete]
	public static void RequestClickAction(this IItemSlot slot) {
        InventoryController inventory = GameManager.instance.Player.Inventory;
		InputHandler.RequestAction(new("ItemDrag", 10, () => {
			InvocationHelper.If(slot.ValidClickAction(inventory.GrabbedItem), () => slot.OnClick(inventory.GrabbedItem, inventory));
		}, null));
		InputHandler.BlockContext("ItemUse", () => !GameManager.instance.Player.InputHandler.LeftHold);
	}

	[Obsolete]
	public static bool ValidClickAction(this IItemSlot slot, ItemDisplay? grabbedItem) => grabbedItem != null || slot.HasItem;

	[Obsolete]
	public static void TransferGrabbed(this IItemSlot slot, ItemDisplay? grabbedItem, InventoryController inventory) {
		if (!inventory.PopupOpen) {
			return;
		}
		PlayerController player = GameManager.instance.Player;
		void SetDropCapabilities(bool enabled) {
			if (!enabled) {
				player.ItemUsageHandler.Disable(ItemUseTrigger.RightClick, ItemUseTrigger.RightHold);
				player.InputHandler.inputActions.Player.RightClick.performed += actionContext => inventory.DropGrabbedItem();
			} else {
				player.ItemUsageHandler.Enable(ItemUseTrigger.RightClick, ItemUseTrigger.RightHold);
				player.InputHandler.inputActions.Player.RightClick.performed -= actionContext => inventory.DropGrabbedItem();
			}
		}

		//Debug.Log(grabbedItem?.ItemStack +", "+ slot.ItemStack + ", "+ (grabbedItem?.ItemStack.Item == slot.ItemStack?.Item));

		// Grab item
		if (grabbedItem == null && slot.HasItem) { 
			SetDropCapabilities(false);
			player.SetMainHandItem(slot.ItemStack!);
			inventory.GrabbedItem = slot.ItemDisplay;
			slot.DetachItemDisplay();
			return;
		} 

		// Release item
		if (grabbedItem != null && slot.IsEmpty) {
			SetDropCapabilities(true);
			(slot as IItemSlot).AttachItemDisplay(inventory.GrabbedItem);
			player.SetMainHandItem(inventory.Hotbar.ActiveSlot.ItemStack);
			inventory.GrabbedItem = null;
			return;
		}
		
		// Swap items
		if (slot.ItemStack?.Item != inventory.GrabbedItem.ItemStack.Item || inventory.GrabbedItem.ItemStack.HasMaxStack() || slot.ItemStack.HasMaxStack()) {
			ItemDisplay previousGrabbed = inventory.GrabbedItem;
			inventory.GrabbedItem = slot.ItemDisplay;
			slot.DetachItemDisplay();
			slot.AttachItemDisplay(previousGrabbed);
			player.SetMainHandItem(inventory.GrabbedItem.ItemStack);
		} else {	// Merge/flow items
			int space = slot.ItemStack.Item.maxStackSize - slot.ItemStack.Quantity;
			int transfer = Math.Min(space, inventory.GrabbedItem.ItemStack.Quantity); 
			slot.ItemStack.Quantity += transfer;
			inventory.GrabbedItem.ItemStack.Quantity -= transfer;
			if (inventory.GrabbedItem.ItemStack.Quantity <= 0) {
				SetDropCapabilities(true);
				inventory.GrabbedItem.Destroy();
				inventory.GrabbedItem = null;
			}
		}
	}
}
