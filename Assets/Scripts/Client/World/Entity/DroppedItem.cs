using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DroppedItem : Entity {
	public const float defaultLifespanSeconds = 120f;			// TODO: decide on a dropped item lifespan duration

	public ItemStack ItemStack { get; set; }
	private float despawnTimer;
	private bool isFrozen;

	public float pickupDelay;
	private float pickupTimer;

	public override void Spawn(EntitySpawnData spawnData) {
		base.ValidateSpawnData<DroppedItemSpawnData>(spawnData, (spawnData) => {
			this.ItemStack = spawnData.itemStack;
			this.pickupDelay = spawnData.pickupDelay;
			this.transform.position = spawnData.position;
			Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
			rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
			BoxCollider2D pickupHitbox = gameObject.AddComponent<BoxCollider2D>();
			pickupHitbox.isTrigger = true;
			pickupHitbox.callbackLayers = LayerMask.GetMask("Player");
			gameObject.AddComponent<BoxCollider2D>().excludeLayers = ~LayerMask.GetMask("Ground");
			Vector2 force = new Vector2(Random.Range(1f, 1.5f), Random.Range(1f, 1.5f)) * spawnData.dropForce;
			rigidbody.AddForce(force, ForceMode2D.Impulse);
			OnEnable();
		});
	}

	private void OnEnable() {
		pickupTimer = pickupDelay;
		despawnTimer = defaultLifespanSeconds;
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (pickupTimer <= 0) {
			if (GameManager.instance.Player.Inventory.PickUpItem(ItemStack)) {
				GameManager.instance.Level.EntityManager.RemoveEntity(this);
			}
		}
	}

	public override void EntityUpdate(float deltaTime) {
		if (isFrozen) {
			return;
		}
		pickupTimer -= deltaTime;
		despawnTimer -= deltaTime;
		if (despawnTimer <= 0) {
			GameManager.instance.Level.EntityManager.RemoveEntity(this);
		}
	}

	public override void OnChunkLoaded() {
		gameObject.SetActive(true);
		isFrozen = false;
	}

	public override void OnChunkUnloaded() { 
		gameObject.SetActive(false);
		isFrozen = true;
	}

	public override Bounds GetBounds() => this.GetColliderBounds();
}