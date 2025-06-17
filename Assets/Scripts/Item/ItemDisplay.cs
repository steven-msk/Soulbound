using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[Header("Debug Internal")]
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

	private void Update() {
		if (moveMode) {
			gameObject.transform.position = GameManager.GetPlayerInstance().InputHandler.MouseScreenPosition;
		}
		tooltip.Update(itemStack);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (GameManager.GetPlayerInstance().Inventory.PopupOpen) {
			tooltip.Show(gameObject.GetComponent<RectTransform>().anchoredPosition, transform);
			tooltip.DisplayParent = gameObject;
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		tooltip.Hide();
	}

	public void EnableMoveMode() {
		moveMode = true;
		gameObject.GetComponent<Image>().raycastTarget = false;
	}

	public void DisableMoveMode() {
		moveMode = false;
		gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
		gameObject.GetComponent<Image>().raycastTarget = true;
	}
}