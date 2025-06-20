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
				GameManager.GetPlayerInstance().Inventory.DestroyItemDisplay(display);
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

	public GameObject Initialize(ItemDisplay parent) {
		// FIXME: poor stack number positioning

		GameObject stackText = GameObject.Instantiate(Registry.Get<GameObject>("stackNumberPrefab"), parent.transform);
		TextMeshProUGUI text = stackText.GetComponent<TextMeshProUGUI>();
		text.autoSizeTextContainer = true;
		RectTransform rectTransform = stackText.GetComponent<RectTransform>();
		if (Item.IsStackable) {
			text.text = $"{FormatStackCount(Quantity)}";
			InventoryController inventory = GameManager.GetPlayerInstance().Inventory;
			Color textColor = Color.white;
			StorageSlot hotbarSlot = parent.GetComponentInParent<StorageSlot>();
			if (!inventory.PopupOpen && hotbarSlot != null && inventory.Hotbar.ActiveSlot != hotbarSlot) {
				textColor = inventory.Hotbar.inactiveSlotNumberColor;
			}
			text.color = textColor;
		}
		rectTransform.pivot = new Vector2(1f, 0f);
		rectTransform.anchoredPosition = Vector3.zero;
		text.rectTransform.sizeDelta = text.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);
		this.stackText = stackText;
		this.display = parent;
		return stackText;
	}

	public void Drop(bool playerAction = false) {
		// TODO: item drop movement (throw force)
		GameObject pickupItem = GameObject.Instantiate(Item.WorldPrefab, null);
		pickupItem.transform.position = GameManager.GetPlayerInstance().transform.position;
		pickupItem.AddComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.NeverSleep;
		BoxCollider2D pickupHitbox = pickupItem.AddComponent<BoxCollider2D>();
		pickupHitbox.isTrigger = true;
		pickupHitbox.callbackLayers = LayerMask.GetMask("Player");
		pickupItem.AddComponent<BoxCollider2D>().excludeLayers = ~LayerMask.GetMask("Ground");
		DroppedItem pickup = pickupItem.AddComponent<DroppedItem>();
		pickup.ItemStack = this;
		pickup.pickupDelay = playerAction ? 2 : 0;
	}

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
