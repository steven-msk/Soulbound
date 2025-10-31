using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client.World.Entity {
	public class DroppedItem : Entity {
		private static readonly Logger logger = Logger.CreateInstance();
		public const float defaultLifespanSeconds = 120f;           // TODO: decide on a dropped item lifespan duration

		public override Type entityScriptType => typeof(DroppedItem);
		public override string prefabDefinitionID => "droppedItem";
		public ItemStack ItemStack { get; private set; }
		public override float facing { get => 1f; set => _ = value; }

		private float despawnTimer;
		private bool isFrozen;

		public float pickupDelay;
		private float pickupTimer;
		private bool flag_pickupLocked = false;

		[EntitySpawnPropertyCandidates("itemStack", "pickupDelay", "dropForce")]
		public override void Spawn(EntitySpawnData spawnData) {
			base.Spawn(spawnData);
			this.ItemStack = spawnData.Get<ItemStack>("itemStack");
			this.pickupDelay = spawnData.Get<float>("pickupDelay");
			this.ApplyIcon(ItemStack.item.aspect.icon);

			Vector2 dropForce = spawnData.Get<Vector2>("dropForce", Vector2.zero);
			float xForce = UnityEngine.Random.Range(1f, 1.5f);
			float yForce = UnityEngine.Random.Range(1f, 1.5f);
			Vector2 force = new Vector2(spawnData.Contains("dropForce") ? xForce : 0f, yForce) * dropForce;
			GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
			OnEnable();
		}

		public void ApplyIcon(ItemIcon icon) {
			SpriteRenderer spriteRenderer = ComponentUtility.GetOrAddComponent<SpriteRenderer>(gameObject);
			spriteRenderer.sprite = icon.sprite;

			float scale = (float)icon.importedPixelsPerUnit / icon.intendedPixelsPerUnit;
			transform.localScale = new Vector3(scale, scale, scale);

			Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
			rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;

			PolygonCollider2D pickupHitbox = gameObject.AddComponent<PolygonCollider2D>();
			pickupHitbox.autoTiling = true;
			pickupHitbox.isTrigger = true;
			pickupHitbox.callbackLayers = LayerMask.GetMask("Player");

			PolygonCollider2D groundCollider = gameObject.AddComponent<PolygonCollider2D>();
			groundCollider.autoTiling = true;
			groundCollider.excludeLayers = ~LayerMask.GetMask("Ground");

		}

		public override SerializedEntityPropertyList GetSerializedProperties() {
			return new SerializedEntityPropertyList()
				.Add("itemStack", this.ItemStack)
				.Add("pickupDelay", this.pickupDelay)
				.Add("pickupTimer", this.pickupTimer)
				.Add("despawnTimer", this.despawnTimer)
				.Add("isFrozen", this.isFrozen)
				.Add("flag_pickupLocked", this.flag_pickupLocked);
		}

		public override void ApplySerializedProperties(SerializedEntityPropertyList properties) {
			this.ItemStack = properties.Get<ItemStack>("itemStack");
			this.pickupDelay = properties.Get<float>("pickupDelay");
			this.pickupTimer = properties.Get<float>("pickupTimer");
			this.despawnTimer = properties.Get<float>("despawnTimer");
			this.isFrozen = properties.Get<bool>("isFrozen");
			this.flag_pickupLocked = properties.Get<bool>("flag_pickupLocked");
		}

		private void OnEnable() {
			pickupTimer = pickupDelay;
			despawnTimer = defaultLifespanSeconds;
		}

		private void OnTriggerStay2D(Collider2D collision) {
			if (pickupTimer <= 0 && !flag_pickupLocked) {
				if (Soulbound.instance.GetActiveLevel().Player.Inventory.PickUpItem(ItemStack, out int remaining)) {
					flag_pickupLocked = true;
					Soulbound.instance.GetActiveLevel().EntityManager.RemoveEntityImmediately(this, destroy: true);
				} else {
					ItemStack.SetQuantity(remaining);
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
}