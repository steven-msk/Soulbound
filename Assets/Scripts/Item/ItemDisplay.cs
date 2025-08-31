using Mono.Cecil;
using System;
using Unity.VisualScripting;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable

public class ItemDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
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
	public TooltipRenderer tooltipRenderer { get; private set; }
	public Item? DisplayedItem => ItemStack?.item;
	private Tooltip? activeTooltip = null;

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
		display.tooltipRenderer = new TooltipRenderer(TooltipNodeStylePresets.PresetProvider());
		display.transform.SetAsLastSibling();
		return display;
	}

	private void Update() {
		InputHandler inputHandler = GameManager.instance.Player.InputHandler;
		if (moveMode) {
			gameObject.transform.position = inputHandler.MouseScreenPosition;
		}
		activeTooltip?.SetPosition(inputHandler.MouseScreenPosition);
	}

	public void Destroy() {
		if (ItemStack == GameManager.instance.Player.MainHandStack) {
			GameManager.instance.Player.SetMainHandItem(null);
		}
		GameObject.Destroy(gameObject);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		TooltipData? tooltipData = itemStack.item.GetTooltipData();
		if (tooltipData != null) {
			activeTooltip = new Tooltip(tooltipRenderer, tooltipData);
			activeTooltip.Show(eventData.position, this.GetComponent<RectTransform>());
			activeTooltip.SetParent(GameManager.instance.Player.Inventory.GetComponent<RectTransform>(), true);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		activeTooltip?.Hide();
		activeTooltip = null;
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