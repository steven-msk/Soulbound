using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using UnityEditor.Profiling;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client.World.EntitySystem {
	public class DroppedItem : Entity, IUpdatable, IChunkListener, IEntitySpawnable<DroppedItem.SpawnData> {
		private static readonly Logger logger = Logger.CreateInstance();
		public const float defaultLifespanSeconds = 120f;           // TODO: decide on a dropped item lifespan duration
		public override EntityDescriptor descriptor => EntityDescriptorRegistry.ByType<DroppedItem>();
		public override Type scriptType => typeof(DroppedItem);
		public ItemStack itemStack { get; private set; }
		public override Facing facing => Facing.Left;

		private float despawnTimer;
		private bool isFrozen;

		public float pickupDelay;
		private float pickupTimer;
		private bool flag_pickupLocked = false;

		void IEntitySpawnable<SpawnData>.ApplySpawnData(SpawnData spawnData) {
			this.transform.position = spawnData.position;
			this.itemStack = spawnData.itemStack;
			this.pickupDelay = spawnData.pickupDelay;
			this.ApplyIcon(itemStack.item.aspect.icon);

			Vector2 dropForce = spawnData.dropForce;
			float xForce = UnityEngine.Random.Range(1f, 1.5f);
			float yForce = UnityEngine.Random.Range(1f, 1.5f);
			Vector2 force = new Vector2(xForce, yForce) * dropForce;
			GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

			pickupTimer = pickupDelay;
			despawnTimer = defaultLifespanSeconds;
		}

		[PROTOTYPICAL]
		private void ApplyIcon(ItemIcon icon) {
			SpriteRenderer spriteRenderer = ComponentUtility.GetOrAddComponent<SpriteRenderer>(gameObject);
			var sprite = AssetManager.Resolve<Sprite>(itemStack.item.aspect.icon.spriteKey);
			spriteRenderer.sprite = sprite;

			float scale = (float)sprite.pixelsPerUnit / icon.intendedPixelsPerUnit;
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

		public override SerializedEntity Serialize() {
			var serialized = base.Serialize();

			var properties = SerializedEntityPropertyList.From(serialized.properties);
			properties.Set(nameof(despawnTimer), despawnTimer);
			properties.Set(nameof(itemStack), itemStack);
			properties.Set(nameof(isFrozen), isFrozen);
			serialized.properties = properties;

			return serialized;
		}

		public override void Deserialize(SerializedEntity serialized) {
			base.Deserialize(serialized);
			var properties = SerializedEntityPropertyList.From(serialized.properties);
			this.despawnTimer = properties.Get<float>(nameof(despawnTimer));
			this.itemStack = properties.Get<ItemStack>(nameof(itemStack));
			this.ApplyIcon(itemStack.item.aspect.icon);
			this.isFrozen = properties.Get<bool>(nameof(isFrozen));
		}

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