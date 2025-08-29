using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Graphs;
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
	public Item? ContainedItem => ItemStack?.item;
	public GameObject GameObject { get; }
		
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
		this.ItemStack!.SetQuantity(quantity);
	}

	public int TryAddStack(int add, Item fallback) {
		if (!CreateDisplayIfEmpty(new ItemStack(fallback, 0))) {
			return this.ItemStack!.Increment(add);
		}
		this.ItemStack!.Increment(add);
		return add;
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

	/// <summary>
	/// Validates whether this slot agrees to interact with the given item upon the given interaction mode
	/// </summary>
	virtual bool Handshake(ItemDisplay? grabbedItem, SlotInteractionMode interactionMode) {
		return interactionMode == SlotInteractionMode.Click ? !(grabbedItem == null && this.IsEmpty) : true;
	}


	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => this.OnPointerDown(eventData);
	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => this.OnPointerUp(eventData);
	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => this.OnPointerEnter(eventData);
}