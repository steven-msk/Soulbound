using Mono.Cecil;
using System;
using Unity.VisualScripting;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[Header("Internal")]
	[SerializeField] private bool moveMode;
	[SerializeField] private Item displayedItem;
	private ItemStack itemStack;
	public ItemStack ItemStack {
		get => itemStack;
		set {
			itemStack = value;
			gameObject.GetComponent<Image>().sprite = itemStack.item.icon;
			displayedItem = itemStack.item;
		}
	}
	public Item? DisplayedItem => ItemStack?.item;
	[SerializeField] private AbstractTooltip tooltip;
	public AbstractTooltip Tooltip { get => tooltip; set => tooltip = value; }

#nullable enable
	public static ItemDisplay Create<TSlot>(ItemStack itemStack, TSlot? slot) where TSlot : MonoBehaviour, IItemSlot {
		return Create(itemStack, () => slot?.transform ?? null);
	}

	public static ItemDisplay Create<TSlot>(Item item, int quantity, TSlot? slot) where TSlot : MonoBehaviour, IItemSlot {
		return Create(new ItemStack(item, quantity), slot);
	}

	public static ItemDisplay Create(ItemStack itemStack, Func<Transform?> parentSupplier) {
		GameObject? obj = Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("itemDisplayPrefab"), parentSupplier.Invoke());
		ItemDisplay? display = obj?.GetComponent<ItemDisplay>();
		UnityEngine.Debug.Assert(display != null, $"ItemDisplay component not found in item display prefab");
		display!.ItemStack = itemStack;
		if (itemStack.item.IsStackable) {
			itemStack.InitializeStackText(display);
		}
		display.Tooltip = itemStack.item.GetTooltip();
		display.transform.SetAsLastSibling();
		return display;
	}
#nullable disable

	private void Update() {
		if (moveMode) {
			gameObject.transform.position = Input.mousePosition;
		}
		if (tooltip?.IsDisplayed ?? false) {
			tooltip.Update(itemStack);
		}
	}

	public void Destroy() {
		if (ItemStack == GameManager.instance.Player.MainHandStack) {
			GameManager.instance.Player.SetMainHandItem(null);
		}
		GameObject.Destroy(gameObject);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		tooltip.Show(eventData.position, this.GetComponent<RectTransform>());
		tooltip.DisplayParent = gameObject;
	}

	public void OnPointerExit(PointerEventData eventData) {
		tooltip.Hide();
	}

	public void EnableGrab() {
		moveMode = true;
		gameObject.GetComponent<Image>().raycastTarget = false;
	}

	public void DisableGrab() {
		moveMode = false;
		gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
		gameObject.GetComponent<Image>().raycastTarget = true;
	}
}