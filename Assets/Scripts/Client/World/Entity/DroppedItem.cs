using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DroppedItem : Entity {
	private static readonly Logger logger = Logger.CreateInstance();
	public const float defaultLifespanSeconds = 120f;			// TODO: decide on a dropped item lifespan duration

	public override Type entityScriptType => typeof(DroppedItem);
	public override string prefabDefinitionID => "droppedItem";
	public string spriteID { get; private set; }
	public ItemStack ItemStack { get; private set; }
	private float despawnTimer;
	private bool isFrozen;

	public float pickupDelay;
	private float pickupTimer;
	private bool flag_pickupLocked = false;

	[EntitySpawnPropertyCandidates("itemStack", "pickupDelay", "dropForce", "spriteID")]
	public override void Spawn(EntitySpawnData spawnData) {
		base.Spawn(spawnData);
		InvocationHelper.If(!spawnData.Contains("spriteID"), 
			() => logger.LogError(null, "Could not find spriteID for dropped item with id {}", this.id));
		this.spriteID = spawnData.Get<string>("spriteID", null);
		GetComponent<SpriteRenderer>().sprite = spriteID != null ? ResourceManager.Get<Sprite, ResourceGroups.Items.Icons>(spriteID) : null;
		this.ItemStack = spawnData.Get<ItemStack>("itemStack");
		this.pickupDelay = spawnData.Get<float>("pickupDelay");
		Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
		rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
		BoxCollider2D pickupHitbox = gameObject.AddComponent<BoxCollider2D>();
		pickupHitbox.isTrigger = true;
		pickupHitbox.callbackLayers = LayerMask.GetMask("Player");
		gameObject.AddComponent<BoxCollider2D>().excludeLayers = ~LayerMask.GetMask("Ground");
		Vector2 dropForce = spawnData.Get<Vector2>("dropForce", Vector2.zero);
		float xForce = UnityEngine.Random.Range(1f, 1.5f);
		float yForce = UnityEngine.Random.Range(1f, 1.5f);
		Vector2 force = new Vector2(spawnData.Contains("dropForce") ? xForce : 0f, yForce) * dropForce;
		rigidbody.AddForce(force, ForceMode2D.Impulse);
		OnEnable();
	}

	public override SerializedEntityPropertyList GetSerializedProperties() {
		return new SerializedEntityPropertyList()
			.Add("itemStack", this.ItemStack)
			.Add("pickupDelay", this.pickupDelay)
			.Add("pickupTimer", this.pickupTimer)
			.Add("despawnTimer", this.despawnTimer)
			.Add("isFrozen", this.isFrozen)
			.Add("flag_pickupLocked", this.flag_pickupLocked)
			.Add("spriteID", this.spriteID);
	}

	public override void ApplySerializedProperties(SerializedEntityPropertyList properties) {
		this.ItemStack = properties.Get<ItemStack>("itemStack");
		this.pickupDelay = properties.Get<float>("pickupDelay");
		this.pickupTimer = properties.Get<float>("pickupTimer");
		this.despawnTimer = properties.Get<float>("despawnTimer");
		this.isFrozen = properties.Get<bool>("isFrozen");
		this.flag_pickupLocked = properties.Get<bool>("flag_pickupLocked");
		this.spriteID = properties.Get<string>("spriteID");
	}

	private void OnEnable() {
		pickupTimer = pickupDelay;
		despawnTimer = defaultLifespanSeconds;
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (pickupTimer <= 0 && !flag_pickupLocked) {
			if (GameManager.instance.Player.Inventory.PickUpItem(ItemStack)) {
				flag_pickupLocked = true;
				GameManager.instance.Level.EntityManager.RemoveEntityImmediately(this, destroy: true);
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