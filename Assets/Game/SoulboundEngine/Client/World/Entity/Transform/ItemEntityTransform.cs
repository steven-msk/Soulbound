using SoulboundEngine.Client.ItemSystem;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {

	[RequireComponent(typeof(Rigidbody2D))]
	public class ItemEntityTransform : MonoBehaviour, Client.Render.Entity.IEntityView {
		private ItemEntity entity = null!;
		private Entity? owner;
		private float spawnTime;
		private Rigidbody2D rb = null!;

		public void Init(ItemEntity entity) {
			this.entity = entity;
			this.rb = this.GetComponent<Rigidbody2D>();
			this.spawnTime = Time.unscaledTime;
			this.owner = this.entity.GetOwner();
		}

		public void Destroy() => Destroy(this.gameObject);

		public Vector2 GetPos() => this.rb.position;

		public void SetPos(Vector2 position) => this.rb.position = position;

		private void OnTriggerStay2D(Collider2D collider) {
			if (this.TryPickup(collider)) {
				this.entity.Destroy();
			}
		}

		private bool TryPickup(Collider2D collider) {
			if (!collider.TryGetComponent<IItemCollector>(out var itemCollector)) {
				return false;
			}
			if (!this.CanBePickedUp(itemCollector.GetEntity())) return false;

			return itemCollector.TryPickupStack(this.entity.GetStack());
		}

		private bool CanBePickedUp(Entity? collidedEntity) {
			if (collidedEntity != this.owner) return true;
			return Time.unscaledTime > this.spawnTime + ItemEntity.CANNOT_PICK_UP_DELAY_SEC;
		}

		public GameObject GetGameObject() => this.gameObject;
		public void SetVisible(bool visible) => this.gameObject.SetActive(visible);
	}
}
