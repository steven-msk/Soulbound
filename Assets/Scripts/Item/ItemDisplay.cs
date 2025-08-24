using Mono.Cecil;
using Unity.VisualScripting;
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
			gameObject.GetComponent<Image>().sprite = itemStack.Item.icon;
			displayedItem = itemStack.Item;
		}
	}
	[SerializeField] private AbstractTooltip tooltip;
	public AbstractTooltip Tooltip { get => tooltip; set => tooltip = value; }

#nullable enable
	public static ItemDisplay Create<TSlot>(ItemStack itemStack, TSlot? slot) where TSlot : MonoBehaviour, IItemSlot {
		GameObject? obj = Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("itemDisplayPrefab"), slot?.transform ?? null);
		ItemDisplay? display = obj?.GetComponent<ItemDisplay>();
		UnityEngine.Debug.Assert(display != null, $"ItemDisplay component not found in item display prefab");
		display!.ItemStack = itemStack;
		if (itemStack.Item.IsStackable) {
			itemStack.InitializeStackText(display);
		}
		display.Tooltip = itemStack.Item.GetTooltip();
		return display;
	}

	public static ItemDisplay Create<TSlot>(Item item, int quantity, TSlot? slot) where TSlot : MonoBehaviour, IItemSlot {
		ItemStack itemStack = new(item, quantity);
		return Create(itemStack, slot);
	}
#nullable disable

	private void Start() {
		transform.SetAsFirstSibling();
	}

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