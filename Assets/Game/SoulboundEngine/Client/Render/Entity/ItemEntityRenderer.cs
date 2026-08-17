using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.World.Entity;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class ItemEntityRenderer : EntityRenderer<ItemEntity, ItemEntityRenderState, ItemEntityModel> {
		private readonly ItemRenderManager itemRenderManager;

		public ItemEntityRenderer(FactoryContext context) 
			: base(context) {
			this.itemRenderManager = context.itemRenderManager;
		}

		public override ItemEntityRenderState CreateRenderState(ItemEntity entity) {
			return new ItemEntityRenderState {
				descriptor = EntityType.ITEM,
				entity = entity,
				stack = entity.GetStack()
			};
		}

		public override EntityViewHandle Create(ItemEntityRenderState state, ItemEntityModel model) {
			ItemRenderer itemRenderer = this.itemRenderManager.GetRenderer(state.stack.GetItem());
			ItemModel itemModel = this.itemRenderManager.GetModel(state.stack);

			ItemRenderContext renderContext = new ItemRenderContext.World { position = state.entity.GetPosition() };
			object itemRenderState = itemRenderer.CreateRenderStateBoxed(state.stack, renderContext);
			IItemView itemView = itemRenderer.CreateViewBoxed(itemRenderState, itemModel, renderContext);
			if (!itemView.IsValid()) throw new InvalidOperationException("Cannot create entity view from invalid item view");

			GameObject obj = ((IItemView.GameObjectBacked)itemView).GetGameObject();
			return EntityViewHandle.Of(obj);
		}
	}
}
