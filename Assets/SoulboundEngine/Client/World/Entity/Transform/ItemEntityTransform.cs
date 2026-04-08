using SoulboundEngine.Client.ItemSystem;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Transform {
	[RequireComponent(typeof(Rigidbody2D))]
	[Obsolete]
	public class ItemEntityTransform : MonoBehaviour, IEntityTransform {
		// currently the transform leaves the implementation hidden for physics transforms.
		// this encapsulation doesnt match the default way of entities to express their state.
		// with this being changed, ItemEntity will be able to encapsulate ItemEntityTransform logic.
		// this separation will become important for headless simulations later on

		private ItemEntity entity = null!;
		private Entity? owner;
		private float pickupDelaySec;
		private float spawnTime;
		private Rigidbody2D rb = null!;

		public void Bind(Entity entity) {
			this.entity = (ItemEntity)entity;
			rb = GetComponent<Rigidbody2D>();
			spawnTime = Time.unscaledTime;
			pickupDelaySec = this.entity.GetPickupDelay();
			owner = this.entity.GetOwner();
		}

		public void Destroy() => Destroy(gameObject);

		Entity IEntityTransform.GetEntity() => entity;
		public ItemEntity GetEntity() => entity;

		public Vector2 GetPos() => rb.position;

		public void SetPos(Vector2 position) => rb.position = position;

		private void OnTriggerStay2D(Collider2D collider) {
			if (TryPickup(collider)) {
				entity.Destroy();
			}
		}

		private bool TryPickup(Collider2D collider) {
			if (!collider.TryGetComponent<IItemPickupHandler>(out var pickupHandler)) {
				return false;
			}
			bool entityTrigger = collider.TryGetComponent(out IEntityTransform entityTransform);
			Entity? collidedEntity = entityTrigger ? entityTransform.GetEntity() : null;
			if (!CanBePickedUp(collidedEntity)) return false;

			return pickupHandler.TryPickupStack(entity.GetStack());
		}

		private bool CanBePickedUp(Entity? collidedEntity) {
			if (collidedEntity != owner) return true;
			return Time.unscaledTime > spawnTime + pickupDelaySec;
		}

		void IEntityTransform.FrameUpdate() {
		}
	}
}
