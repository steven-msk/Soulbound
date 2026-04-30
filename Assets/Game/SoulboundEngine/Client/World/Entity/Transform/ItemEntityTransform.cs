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
		private float spawnTime;
		private Rigidbody2D rb = null!;

		public void Bind(Entity entity) {
			this.entity = (ItemEntity)entity;
			this.rb = this.GetComponent<Rigidbody2D>();
			this.spawnTime = Time.unscaledTime;
			this.owner = this.entity.GetOwner();
		}

		public void Destroy() => Destroy(this.gameObject);

		Entity IEntityTransform.GetEntity() => this.entity;
		public ItemEntity GetEntity() => this.entity;

		public Vector2 GetPos() => this.rb.position;

		public void SetPos(Vector2 position) => this.rb.position = position;

		private void OnTriggerStay2D(Collider2D collider) {
			if (this.TryPickup(collider)) {
				this.entity.Destroy();
			}
		}

		private bool TryPickup(Collider2D collider) {
			if (!collider.TryGetComponent<IItemPickupHandler>(out var pickupHandler)) {
				return false;
			}
			bool entityTrigger = collider.TryGetComponent(out IEntityTransform entityTransform);
			Entity? collidedEntity = entityTrigger ? entityTransform.GetEntity() : null;
			if (!this.CanBePickedUp(collidedEntity)) return false;

			return pickupHandler.TryPickupStack(this.entity.GetStack());
		}

		private bool CanBePickedUp(Entity? collidedEntity) {
			if (collidedEntity != this.owner) return true;
			return Time.unscaledTime > this.spawnTime + ItemEntity.CANNOT_PICK_UP_DELAY_SEC;
		}

		void IEntityTransform.FrameUpdate() {
		}
	}
}
