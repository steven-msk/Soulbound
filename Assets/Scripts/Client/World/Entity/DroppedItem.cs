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
		base.Spawn(spawnData);
		this.ItemStack = spawnData.Get<ItemStack>("itemStack");
		this.pickupDelay = spawnData.Get<float>("pickupDelay");
		Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
		rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
		BoxCollider2D pickupHitbox = gameObject.AddComponent<BoxCollider2D>();
		pickupHitbox.isTrigger = true;
		pickupHitbox.callbackLayers = LayerMask.GetMask("Player");
		gameObject.AddComponent<BoxCollider2D>().excludeLayers = ~LayerMask.GetMask("Ground");
		Vector2 force = new Vector2(Random.Range(1f, 1.5f), Random.Range(1f, 1.5f)) * spawnData.Get<Vector2>("dropForce");
		rigidbody.AddForce(force, ForceMode2D.Impulse);
		OnEnable();
	}

	private void OnEnable() {
		pickupTimer = pickupDelay;
		despawnTimer = defaultLifespanSeconds;
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (pickupTimer <= 0) {
			if (GameManager.instance.Player.Inventory.PickUpItem(ItemStack)) {
				this.Despawn();
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
			this.Despawn();
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