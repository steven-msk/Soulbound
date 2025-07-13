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
			gameObject.GetComponent<Image>().sprite = itemStack.Item.Icon;
			displayedItem = itemStack.Item;
		}
	}
	[SerializeField] private AbstractTooltip tooltip;
	public AbstractTooltip Tooltip { get => tooltip; set => tooltip = value; }

	private void Start() {
		transform.SetAsFirstSibling();
	}

	private void Update() {
		if (moveMode) {
			gameObject.transform.position = GameManager.instance.Player.InputHandler.MouseScreenPosition;
		}
		if (tooltip?.IsDisplayed ?? false) {
			tooltip.Update(itemStack);
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (GameManager.instance.Player.Inventory.PopupOpen) {
			tooltip.Show(eventData.position, transform);
			tooltip.DisplayParent = gameObject;
		}
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