using UnityEngine;
using UnityEngine.UI;

public class DroppedItem : MonoBehaviour {

	public ItemStack ItemStack { get; set; }
	public float pickupDelay = 2f;
	private float pickupTimer = 0;

	private void OnEnable() {
		pickupTimer = pickupDelay;
	}

	private void Update() {
		pickupTimer -= Time.deltaTime;
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (pickupTimer <= 0) {
			if (GameManager.instance.Player.Inventory.GrabItem(ItemStack)) {
				Destroy(gameObject);
			}
		}
	}
}