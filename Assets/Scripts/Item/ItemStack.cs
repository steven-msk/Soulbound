using System;
using TMPro;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ItemStack {
	public Item Item { get; }
	private ItemDisplay display;
	private GameObject stackText;
	private int quantity;
	public int Quantity {
		get => quantity;
		set {
			quantity = value;
			if (quantity <= 0) {
				GameManager.instance.Player.Inventory.DestroyItemDisplay(display);
			}
			if(stackText != null && quantity > 0) {
				UpdateText();
			}
		}
	}

	public ItemStack(Item item, int quantity) {
		this.Item = item;
		this.Quantity = quantity;
	}

	public void UpdateText() {
		TextMeshProUGUI stackText = this.stackText.GetComponent<TextMeshProUGUI>();
		stackText.text = FormatStackCount(Quantity);
	}

	public GameObject InitializeStackText(ItemDisplay parent) {
		GameObject stackText = GameObject.Instantiate(AssetRegistry.Get<GameObject>("stackNumberPrefab"), parent.transform);
		TextMeshProUGUI text = stackText.GetComponent<TextMeshProUGUI>();
		text.autoSizeTextContainer = true;
		RectTransform rectTransform = stackText.GetComponent<RectTransform>();
		if (Item.IsStackable) {
			text.text = $"{FormatStackCount(Quantity)}";
			InventoryController inventory = GameManager.instance.Player.Inventory;
			Color textColor = Color.white;
			InventorySlot hotbarSlot = parent.GetComponentInParent<InventorySlot>();
			if (!inventory.PopupOpen && hotbarSlot != null && inventory.Hotbar.ActiveSlot != hotbarSlot) {
				textColor = inventory.Hotbar.inactiveSlotNumberColor;
			}
			text.color = textColor;
		}
		// Incoming hard-coded values, beware!
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

	public void Drop(Vector2 pos, bool playerAction = false) {
		// FUTURE TODO: item drop movement (throw force)
		GameObject pickupItem = GameObject.Instantiate(Item.WorldPrefab, null);
		DroppedItem pickup = pickupItem.AddComponent<DroppedItem>();
		pickup.Init(this, playerAction ? 2f : 0f, pos);
    }

	// FEATUREIMPL: dropped item stacks converging to avoid lag

	// POTENTIAL REWORK: item max stack numbers
	public static string FormatStackCount(int amount) {
		if (amount < 1000) {
			return amount.ToString();
		}
		if (amount > 999_999) {
			Debug.LogWarning($"Stack size exceeded max limit: {amount}"); 
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
