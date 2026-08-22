using SoulboundEngine.UnityClient.Render.Item;
using SoulboundEngine.World.Entity;
using System;
using UnityEngine;

namespace SoulboundEngine.UnityClient.Render.Entity {
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
			object itemRenderState = itemRenderer.InternalCreateRenderState(state.stack, renderContext);
			ItemViewHandle itemView = itemRenderer.InternalCreate(itemRenderState, itemModel, renderContext);
			if (!itemView.IsValid()) throw new InvalidOperationException("Cannot create entity view from invalid item view");

			GameObject obj = ((ItemViewHandle.GameObjectBacked)itemView).gameObject;
			return EntityViewHandle.Of(obj);
		}
	}
}
