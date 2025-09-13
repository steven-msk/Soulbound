using TMPro;
using UnityEngine;

#nullable enable

public class ItemStack {
	private static readonly Logger logger = Logger.CreateInstance();
	public Item item { get; }
	private ItemDisplay? display; 
	private GameObject? stackText;
	public int quantity { get; private set; }
	public int MaxStackSize => item.maxStackSize;

	public ItemStack(Item item, int quantity) {
		this.item = item;
		this.quantity = Mathf.Min(quantity, item.maxStackSize);
	}

	public void UpdateText() {
		TextMeshProUGUI? stackText = this.stackText?.GetComponent<TextMeshProUGUI>();
		stackText!.text = quantity.ToString();
	}

	public GameObject AssignDisplay(ItemDisplay parent) {
		GameObject? stackText = GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("stackNumberPrefab"), parent.transform);
		TextMeshProUGUI? text = stackText!.GetComponent<TextMeshProUGUI>();
		text.enabled = item.IsStackable;
		RectTransform rectTransform = stackText!.GetComponent<RectTransform>();
		if (item.IsStackable) {
			text!.autoSizeTextContainer = true;
			text.text = quantity.ToString();
			InventoryController inventory = GameManager.instance.Player.Inventory;
			Color textColor = Color.white;
			InventorySlot hotbarSlot = parent.GetComponentInParent<InventorySlot>();
			if (!inventory.IsOpened && hotbarSlot != null && inventory.Hotbar.ActiveSlot != hotbarSlot) {
				textColor = inventory.Hotbar.inactiveSlotNumberColor;
			}
			text.color = textColor;
			rectTransform.pivot = new Vector2(1f, 0f);
			rectTransform.anchorMax = new Vector2(0.9375f, 0.0625f);
			rectTransform.anchorMin = rectTransform.anchorMax;
			rectTransform.anchoredPosition = Vector2.zero;
			text.rectTransform.sizeDelta = text.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);
			rectTransform.anchoredPosition = new(Mathf.Max(-4, rectTransform.sizeDelta.x - 19.14f), rectTransform.anchoredPosition.y);
		}
		this.stackText = stackText;
		this.display = parent;
		return stackText;
	}

	public void Drop(Vector2 pos, Vector2 dropForce, bool playerAction = false) {
		GameObject? worldPrefab = item.aspect.worldPrefabSupplier?.Invoke();
		if (item.aspect.worldPrefabSupplier == null) {
			logger.LogError(null, "No world prefab supplier for item {}. Using icon fallback.", item);
			worldPrefab = WorldPrefabFactory.FromIcon(item.aspect.icon).Invoke();
		}
		GameObject droppedItem = worldPrefab!;
		DroppedItem pickup = droppedItem.GetComponent<DroppedItem>() ?? droppedItem.AddComponent<DroppedItem>();
		GameManager.instance.Level.EntityManager.SpawnEntity(pickup, new EntitySpawnData(pos) {
			[SpawnDataKey.Of("itemStack")] = new SpawnDataValue<ItemStack>(this),
			[SpawnDataKey.Of("pickupDelay")] = new SpawnDataValue<float>((playerAction ? 2f : 0f)),
			[SpawnDataKey.Of("dropForce")] = new SpawnDataValue<Vector2>(dropForce),
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
		if (@new <= 0) {
			display?.Destroy();
		}
		if (stackText != null && @new > 0) {
			UpdateText();
		}
	}

	// FEATUREIMPL: dropped item stacks converging to avoid lag
}
