using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public class ItemEntity : Entity {
		private static readonly EntityDescriptor DESCRIPTOR = new("itemEntity", null);

		private readonly ItemStack itemStack;

		public ItemEntity(ItemStack itemStack, Vector2 initialPos)
			: base(DESCRIPTOR, initialPos) {
			this.itemStack = itemStack;
		}

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Item Entity");
			ItemEntityTransform transform = obj.AddComponent<ItemEntityTransform>();

			Rigidbody2D rigidbody = obj.AddComponent<Rigidbody2D>();
			rigidbody.bodyType = RigidbodyType2D.Kinematic;

			SpriteRenderer itemRenderer = obj.AddComponent<SpriteRenderer>();
			WorldItemDisplayView view = obj.AddComponent<WorldItemDisplayView>();
			view.Init(itemRenderer);
			view.SetStack(itemStack);

			return transform;
		}

		public ItemStack GetStack() => itemStack;
	}
}
