using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client.World.EntitySystem {
	public class DroppedItem : Entity, IUpdatable, IChunkListener {
		private static readonly Logger logger = Logger.CreateInstance();
		public const float defaultLifespanSeconds = 120f;           // TODO: decide on a dropped item lifespan duration

		public override Type scriptType => typeof(DroppedItem);
		public override string prefabDefinitionID => "droppedItem";
		public ItemStack itemStack { get; private set; }
		public override Facing facing => Facing.Left;

		private float despawnTimer;
		private bool isFrozen;

		public float pickupDelay;
		private float pickupTimer;
		private bool flag_pickupLocked = false;

		public override void ApplySpawnData<TData>(TData spawnData) {
			var typed = spawnData as DroppedItem.SpawnData;
			if (typed == null) {
				return;
			}

			this.transform.position = typed.position;
			this.itemStack = typed.itemStack;
			this.pickupDelay = typed.pickupDelay;
			this.ApplyIcon(itemStack.item.aspect.icon);

			Vector2 dropForce = typed.dropForce;
			float xForce = UnityEngine.Random.Range(1f, 1.5f);
			float yForce = UnityEngine.Random.Range(1f, 1.5f);
			Vector2 force = new Vector2(xForce, yForce) * dropForce;
			GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

			pickupTimer = pickupDelay;
			despawnTimer = defaultLifespanSeconds;
		}

		[PROTOTYPICAL]
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

		//public override ComponentSerializer GetSerializedProperties() {
		//	return new ComponentSerializer()
		//		.Add("itemStack", this.ItemStack)
		//		.Add("pickupDelay", this.pickupDelay)
		//		.Add("pickupTimer", this.pickupTimer)
		//		.Add("despawnTimer", this.despawnTimer)
		//		.Add("isFrozen", this.isFrozen)
		//		.Add("flag_pickupLocked", this.flag_pickupLocked);
		//}

		//public override void ApplySerializedProperties(ComponentSerializer properties) {
		//	this.ItemStack = properties.Get<ItemStack>("itemStack");
		//	this.pickupDelay = properties.Get<float>("pickupDelay");
		//	this.pickupTimer = properties.Get<float>("pickupTimer");
		//	this.despawnTimer = properties.Get<float>("despawnTimer");
		//	this.isFrozen = properties.Get<bool>("isFrozen");
		//	this.flag_pickupLocked = properties.Get<bool>("flag_pickupLocked");
		//}

		private void OnTriggerStay2D(Collider2D collision) {
			if (pickupTimer <= 0 && !flag_pickupLocked) {
				if (Soulbound.instance.GetPlayerInstance().Inventory.PickUpItem(itemStack, out int remaining)) {
					flag_pickupLocked = true;
					manager.RemoveEntity(this);
				} else {
					itemStack.SetQuantity(remaining);
				}
			}
		}

		public void FrameUpdate(float deltaTime) {
			if (isFrozen) {
				return;
			}
			pickupTimer -= deltaTime;
			despawnTimer -= deltaTime;
			if (despawnTimer <= 0) {
				manager.RemoveEntity(this);
			}
		}

		public void OnEnteredChunk(WorldChunk chunk) {
			UnityEngine.Debug.Log("chunk load");
			gameObject.SetActive(true);
			isFrozen = false;
		}

		public void OnLeftChunk(WorldChunk chunk) {
			UnityEngine.Debug.Log("chunk unload");
			gameObject.SetActive(false);
			isFrozen = true;
		}

		public sealed class SpawnData : ISpawnData {
			public Vector2 position { get; init; }
			public Vector2 dropForce { get; init; }
			public ItemStack itemStack { get; init; }
			public float pickupDelay { get; init; }
		}
	}
}