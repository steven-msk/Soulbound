using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Client.World.EntitySystem;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class ItemEntityRenderer : EntityRenderer<ItemEntity, ItemEntityRenderState, EntityModel> {
		private readonly ItemRenderManager itemRenderManager;

		public ItemEntityRenderer(FactoryContext context) 
			: base(context) {
			this.itemRenderManager = context.itemRenderManager;
		}

		public override ItemEntityRenderState CreateRenderState(ItemEntity entity) {
			return new ItemEntityRenderState {
				descriptor = ItemEntity.DESCRIPTOR,
				entity = entity,
				stack = entity.GetStack()
			};
		}

		public override IEntityView CreateView(ItemEntityRenderState state, EntityModel model) {
			ItemRenderer itemRenderer = this.itemRenderManager.GetRenderer(state.stack.item);
			ItemModel itemModel = this.itemRenderManager.GetModel(state.stack);

			ItemRenderContext renderContext = new ItemRenderContext.World { position = state.entity.GetPosition() };
			object itemRenderState = itemRenderer.CreateRenderStateBoxed(state.stack, itemModel, renderContext);
			IItemView itemView = itemRenderer.CreateViewBoxed(itemRenderState, renderContext);

			GameObject obj = itemView.GetGameObject();
			ItemEntityTransform transform = obj.AddComponent<ItemEntityTransform>();
			transform.Init(state.entity);

			return transform;
		}

		public override void DestroyView(IEntityView view) {
			view.Destroy();
		}

		public override void UpdateView(ItemEntityRenderState state, IEntityView view) {
		}
	}
}
