using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World.EntitySystem {
	public class ItemEntity : Entity {
		private static readonly EntityDescriptor DESCRIPTOR = new("itemEntity", null);
		private static readonly Vector3 TRANSFORM_SCALE = new(2.5f, 2.5f, 2.5f);

		private readonly Entity? owner;
		private readonly float pickupDelay;
		private readonly ItemStack itemStack;

		public ItemEntity(ItemStack itemStack, Vector2 initialPos)
			: this(null, 0f, itemStack, initialPos) {
		}

		public ItemEntity(Entity? owner, float pickupDelay, ItemStack itemStack, Vector2 initialPos)
			: base(DESCRIPTOR, initialPos) {
			this.itemStack = itemStack;
			this.owner= owner;
			this.pickupDelay = pickupDelay;
		}

		public float GetPickupDelay() => pickupDelay;
		public Entity? GetOwner() => owner;
		public ItemStack GetStack() => itemStack;

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Item Entity");
			ItemEntityTransform transform = obj.AddComponent<ItemEntityTransform>();
			obj.GetComponent<Transform>().localScale = TRANSFORM_SCALE;

			SpriteRenderer itemRenderer = obj.AddComponent<SpriteRenderer>();
			WorldItemDisplayView view = obj.AddComponent<WorldItemDisplayView>();
			view.Init(itemRenderer);
			view.SetStack(itemStack);

			Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();
			rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
			rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

			BoxCollider2D physicsCollider = obj.AddComponent<BoxCollider2D>();
			physicsCollider.excludeLayers = LayerMask.GetMask(Layers.EntityCharacter);

			BoxCollider2D pickupCollider = obj.AddComponent<BoxCollider2D>();
			pickupCollider.isTrigger = true;

			return transform;
		}

		public void Destroy() {
			level.RemoveEntity(this);
			transform.Destroy();
		}
	}
}
