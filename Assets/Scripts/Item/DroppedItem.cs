using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DroppedItem : MonoBehaviour {
	public ItemStack ItemStack { get; set; }
	public float pickupDelay = 0f;
	private float pickupTimer = 0;

	public void Init(ItemStack itemStack, float pickupDelay, Vector2 pos, Vector2 dropForce) {
		this.ItemStack = itemStack;
		this.pickupDelay = pickupDelay;
        this.transform.position = pos;
        Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
        rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        BoxCollider2D pickupHitbox = gameObject.AddComponent<BoxCollider2D>();
        pickupHitbox.isTrigger = true;
        pickupHitbox.callbackLayers = LayerMask.GetMask("Player");
        gameObject.AddComponent<BoxCollider2D>().excludeLayers = ~LayerMask.GetMask("Ground");
		Vector2 force = new Vector2(Random.Range(1f, 1.5f), Random.Range(1f, 1.5f)) * dropForce;
		rigidbody.AddForce(force, ForceMode2D.Impulse);
        OnEnable();
    }

	private void OnEnable() => pickupTimer = pickupDelay;

	private void Update() => pickupTimer -= Time.deltaTime;

	private void OnTriggerStay2D(Collider2D collision) {
		if (pickupTimer <= 0) {
			if (GameManager.instance.Player.Inventory.PickUpItem(ItemStack)) {
				Destroy(gameObject);
			}
		}
	}
}