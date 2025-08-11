using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DroppedItem : MonoBehaviour, IEntity {
	public const float defaultLifespanSeconds = 60f;			// TODO: decide on a dropped item lifespan duration
	public ItemStack ItemStack { get; set; }
	private float despawnTimer;
	private bool isFrozen;
	public System.Guid id { get; set; }
	public Vector2 position { get; set; }

	public float pickupDelay;
	private float pickupTimer;

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
		GameManager.instance.Level.EntityManager.AddEntity(this, pos);
		OnEnable();
	}

	private void OnEnable() {
		pickupTimer = pickupDelay;
		despawnTimer = defaultLifespanSeconds;
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (pickupTimer <= 0) {
			if (GameManager.instance.Player.Inventory.PickUpItem(ItemStack)) {
				Destroy(gameObject);
			}
		}
	}

	void IEntity.Update(float deltaTime) {
		if (isFrozen) {
			return;
		}
		pickupTimer -= deltaTime;
		despawnTimer -= deltaTime;
		if (despawnTimer <= 0) {
			GameManager.instance.Level.EntityManager.RemoveEntity(this);
			Destroy(gameObject);
		}
	}

	public void OnChunkLoaded() {
		gameObject.SetActive(true);
		isFrozen = false;
	}

	public void OnChunkUnloaded() { 
		gameObject.SetActive(false);
		isFrozen = true;
	}
}