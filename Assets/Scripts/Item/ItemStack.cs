using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ItemStack {
	public Item item { get; }
	private ItemDisplay? display; 
	private GameObject stackText;
	public int quantity { get; private set; }
	public int MaxStackSize => item.maxStackSize;

	public ItemStack(Item item, int quantity) {
		this.item = item;
		this.quantity = Mathf.Min(quantity, item.maxStackSize);
	}

	public void UpdateText() {
		TextMeshProUGUI stackText = this.stackText.GetComponent<TextMeshProUGUI>();
		stackText.text = FormatStackCount(quantity);
	}

	public GameObject InitializeStackText(ItemDisplay parent) {
		GameObject stackText = GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("stackNumberPrefab"), parent.transform);
		TextMeshProUGUI text = stackText.GetComponent<TextMeshProUGUI>();
		text.autoSizeTextContainer = true;
		RectTransform rectTransform = stackText.GetComponent<RectTransform>();
		if (item.IsStackable) {
			text.text = $"{FormatStackCount(quantity)}";
			InventoryController inventory = GameManager.instance.Player.Inventory;
			Color textColor = Color.white;
			InventorySlot hotbarSlot = parent.GetComponentInParent<InventorySlot>();
			if (!inventory.PopupOpen && hotbarSlot != null && inventory.Hotbar.ActiveSlot != hotbarSlot) {
				textColor = inventory.Hotbar.inactiveSlotNumberColor;
			}
			text.color = textColor;
		}
		rectTransform.pivot = new Vector2(1f, 0f);
		rectTransform.anchorMax = new Vector2(0.9375f, 0.0625f);
		rectTransform.anchorMin = rectTransform.anchorMax;
		rectTransform.anchoredPosition = Vector2.zero;
		text.rectTransform.sizeDelta = text.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);
		rectTransform.anchoredPosition = new(Mathf.Max(-4, rectTransform.sizeDelta.x - 19.14f), rectTransform.anchoredPosition.y);
		this.stackText = stackText;
		this.display = parent;
		return stackText;
	}

	public void Drop(Vector2 pos, Vector2 dropForce, bool playerAction = false) {
		GameObject worldPrefab = item.worldPrefabSupplier?.Invoke();
		if (item.worldPrefabSupplier == null) {
			UnityEngine.Debug.LogError($"Item '{item}' does not supply world prefab. Using fallback world prefab");
			worldPrefab = item.FallbackWorldPrefab();
		}
		GameObject pickupItem = worldPrefab;
		DroppedItem pickup = pickupItem.GetComponent<DroppedItem>();
		string spriteID = worldPrefab.GetComponent<SpriteRenderer>().sprite.name;
		GameManager.instance.Level.EntityManager.SpawnEntity(pickup, new(pos) {
			[SpawnDataKey.Of("itemStack")] = new SpawnDataValue<ItemStack>(this),
			[SpawnDataKey.Of("pickupDelay")] = new SpawnDataValue<float>((playerAction ? 2f : 0f)),
			[SpawnDataKey.Of("dropForce")] = new SpawnDataValue<Vector2>(dropForce),
			[SpawnDataKey.Of("spriteID")] = new SpawnDataValue<string>(spriteID)
		});
    }

	public bool IsFull() => quantity >= item.maxStackSize;
	public bool IsEmpty() => quantity <= 0;

	public void SetQuantity(int amount) {
		int oldQuantity = quantity;
		this.quantity = Mathf.Min(amount, MaxStackSize);
		OnQuantityChanged(oldQuantity, quantity);
	}

	// Try to add items, return how many were actually added
	public int Increment(int amount = 1) {
		if (amount <= 0) {
			return 0;
		}
		int space = MaxStackSize - quantity;
		int added = Mathf.Min(space, amount);
		quantity += added;
		OnQuantityChanged(quantity - added, quantity);
		return added;
	}

	// Try to remove items, returns how many were actually removed
	public int Decrement(int amount = 1) {
		if (amount <= 0) {
			return 0;
		}
		int removed = Mathf.Min(quantity, amount);
		quantity -= removed;
		OnQuantityChanged(quantity + removed, quantity);
		return removed;
	}

	private void OnQuantityChanged(int old, int @new) {
		if (quantity <= 0) {
			display?.Destroy();
		}
		if (stackText != null && quantity > 0) {
			UpdateText();
		}
	}

	// FEATUREIMPL: dropped item stacks converging to avoid lag

	// POTENTIAL REWORK: item max stack numbers
	public static string FormatStackCount(int amount) {
		if (amount < 1000) {
			return amount.ToString();
		}
		if (amount > 999_999) {
			UnityEngine.Debug.LogWarning($"Stack size exceeded max limit: {amount}"); 
			return "999k";
		}

		float divided = amount / 1000f;
		if (divided >= 999.5f) {
			return "999k";
		}
		if (divided < 9.95f) {
			return divided.ToString("0.#") + "k";
		}
		return Mathf.FloorToInt(divided).ToString() + "k";
	}
}
