using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public class ItemEntity : Entity {
		private static readonly EntityDescriptor DESCRIPTOR = new("itemEntity", null);
		private static readonly Vector3 TRANSFORM_SCALE = new(2.5f, 2.5f, 2.5f);

		private readonly ItemStack itemStack;

		public ItemEntity(ItemStack itemStack, Vector2 initialPos)
			: base(DESCRIPTOR, initialPos) {
			this.itemStack = itemStack;
		}

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Item Entity");
			ItemEntityTransform transform = obj.AddComponent<ItemEntityTransform>();
			obj.GetComponent<Transform>().localScale = TRANSFORM_SCALE;

			SpriteRenderer itemRenderer = obj.AddComponent<SpriteRenderer>();
			WorldItemDisplayView view = obj.AddComponent<WorldItemDisplayView>();
			view.Init(itemRenderer);
			view.SetStack(itemStack);

			Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();
			rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
			BoxCollider2D physicsCollider = obj.AddComponent<BoxCollider2D>();
			physicsCollider.excludeLayers = LayerMask.GetMask(Layers.EntityCharacter);

			return transform;
		}

		public ItemStack GetStack() => itemStack;
	}
}
