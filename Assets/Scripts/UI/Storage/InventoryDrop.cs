using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDrop : MonoBehaviour, IPointerDownHandler {
	public void OnPointerDown(PointerEventData eventData) {
		InputHandler.RequestAction(new("ItemDrop", 10, () => GameManager.GetPlayerInstance().Inventory.OnDropItemTriggered()));
		InputHandler.BlockContextUntil("ItemUse", () => GameManager.GetPlayerInstance().InputHandler.LeftHold);
		gameObject.SetActive(false);
	}
}