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
		this.spriteID = spawnData.Get<string>("spriteID", null);
		GetComponent<SpriteRenderer>().sprite = spriteID != null ? ResourceManager.Get<Sprite, ResourceGroups.Items.Icons>(spriteID) : null;
		InvocationHelper.If(spriteID == null, () => logger.LogError(null, "Could not find spriteID for dropped item with id {}", this.id));
		this.ItemStack = spawnData.Get<ItemStack>("itemStack");
		this.pickupDelay = spawnData.Get<float>("pickupDelay");
		Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
		rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
		BoxCollider2D pickupHitbox = gameObject.AddComponent<BoxCollider2D>();
		pickupHitbox.isTrigger = true;
		pickupHitbox.callbackLayers = LayerMask.GetMask("Player");
		gameObject.AddComponent<BoxCollider2D>().excludeLayers = ~LayerMask.GetMask("Ground");
		Vector2 dropForce = spawnData.Get<Vector2>("dropForce", Vector2.zero);
		Vector2 force = new Vector2(spawnData.Exists("dropForce") ? UnityEngine.Random.Range(1f, 1.5f) : 0f, UnityEngine.Random.Range(1f, 1.5f)) * dropForce;
		rigidbody.AddForce(force, ForceMode2D.Impulse);
		OnEnable();
	}

	public override List<AbstractSerializedEntityProperty> GetSerializedProperties() {
		return new List<AbstractSerializedEntityProperty>() {
			new SerializedEntityProperty<ItemStack>("itemStack", this.ItemStack),
			new SerializedEntityProperty<float>("pickupDelay", this.pickupDelay),
			new SerializedEntityProperty<float>("pickupTimer", this.pickupTimer),
			new SerializedEntityProperty<float>("despawnTimer", this.despawnTimer),
			new SerializedEntityProperty<bool>("isFrozen", this.isFrozen),
			new SerializedEntityProperty<bool>("flag_pickupLocked", this.flag_pickupLocked),
			new SerializedEntityProperty<string>("spriteID", this.spriteID),
		};
	}

	public override void ApplySerializedProperties(List<AbstractSerializedEntityProperty> properties) {
		this.ItemStack = properties.First(p => p.GetKey() == "itemStack").GetValue<ItemStack>();
		this.pickupDelay = properties.First(p => p.GetKey() == "pickupDelay").GetValue<float>();
		this.pickupTimer = properties.First(p => p.GetKey() == "pickupTimer").GetValue<float>();
		this.despawnTimer = properties.First(p => p.GetKey() == "despawnTimer").GetValue<float>();
		this.isFrozen = properties.First(p => p.GetKey() == "isFrozen").GetValue<bool>();
		this.flag_pickupLocked = properties.First(p => p.GetKey() == "flag_pickupLocked").GetValue<bool>();
		this.spriteID = properties.First(p => p.GetKey() == "spriteID").GetValue<string>();
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