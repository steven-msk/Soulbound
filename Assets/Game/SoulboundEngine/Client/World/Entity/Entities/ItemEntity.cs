using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Render.Sprite;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public class ItemEntity : Entity {
		public const float CANNOT_PICK_UP_DELAY_SEC = 2;
		// TODO: add AIR Item
		public static readonly EntityDescriptor<ItemEntity> DESCRIPTOR = EntityDescriptor.Of(
			(_, level) => new ItemEntity(null, level),
			ITransformSupplier<ItemEntity>.Of(entity => {
				GameObject obj = new("Item Entity");
				ItemEntityTransform transform = obj.AddComponent<ItemEntityTransform>();
				obj.GetComponent<UnityEngine.Transform>().localScale = TRANSFORM_SCALE;

				WorldItemView view = entity.itemRenderer.CreateView(obj);
				ItemRenderData renderData = entity.itemStack.item.GetRenderData(entity.itemStack);
				ItemRenderModel model = entity.modelResolver.Resolve(renderData);
				entity.itemRenderer.Render(view, model);

				Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();
				rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
				rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

				BoxCollider2D physicsCollider = obj.AddComponent<BoxCollider2D>();
				physicsCollider.excludeLayers = LayerMask.GetMask(Layers.EntityCharacter);

				BoxCollider2D pickupCollider = obj.AddComponent<BoxCollider2D>();
				pickupCollider.isTrigger = true;

				return transform;
			})
		);

		private static readonly Vector3 TRANSFORM_SCALE = new(2.5f, 2.5f, 2.5f);

		private readonly Entity? owner;
		private readonly float pickupDelaySec;
		private readonly ItemStack itemStack;

		private readonly WorldItemRenderer itemRenderer = new(new AtlasSpriteResolver());
		private readonly ItemModelResolver modelResolver = new();

		public ItemEntity(ItemStack itemStack, Level level)
			: this(null, itemStack, level) {
		}

		public ItemEntity(Entity? owner, ItemStack itemStack, Level level)
			: base(DESCRIPTOR, level) {
			this.itemStack = itemStack;
			this.owner = owner;
		}

		public float GetPickupDelay() => this.pickupDelaySec;
		public Entity? GetOwner() => this.owner;
		public ItemStack GetStack() => this.itemStack;

		public void Destroy() {
			this.level.RemoveEntity(this);
			this.transform.Destroy();
		}
	}
}
