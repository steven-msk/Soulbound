using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Render.Sprite;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public class ItemEntity : Entity {

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
			: this(null, 0f, itemStack, level) {
		}

		public ItemEntity(Entity? owner, float pickupDelaySec, ItemStack itemStack, Level level)
			: base(DESCRIPTOR, level) {
			this.itemStack = itemStack;
			this.owner = owner;
			this.pickupDelaySec = pickupDelaySec;
		}

		public float GetPickupDelay() => pickupDelaySec;
		public Entity? GetOwner() => owner;
		public ItemStack GetStack() => itemStack;

		public void Destroy() {
			level.RemoveEntity(this);
			transform.Destroy();
		}
	}
}
