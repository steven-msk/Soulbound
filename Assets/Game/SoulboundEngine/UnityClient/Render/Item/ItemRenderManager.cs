using SoulboundEngine.UnityClient.Render.Sprite;
using SoulboundEngine.Item;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.UnityClient.Render.Item {
	using Item = SoulboundEngine.Item.Item;

	public sealed class ItemRenderManager {
		private readonly Dictionary<Item, ItemRenderer> renderers;
		private readonly Func<Item, IItemModelResolver> modelResolverFactory;
		private readonly Dictionary<ItemRenderHandle, RenderedItem> rendered = new();

		public ItemRenderManager(List<Item> items, ISpriteResolver<AtlasSpriteRef> spriteResolver) {
			this.modelResolverFactory = ItemRenderers.GetModelResolverFactory(spriteResolver);
			this.renderers = ItemRenderers.LoadRenderers(items);
		}

		public ItemViewHandle? Render(ItemRenderHandle handle, ItemStack stack, ItemRenderContext context) {
			if (this.rendered.ContainsKey(handle)) {
				this.Destroy(handle, context);
			}

			ItemRenderer renderer = this.renderers[stack.GetItem()];
			ItemModel model = this.GetModel(stack);
			object state = renderer.InternalCreateRenderState(stack, context);

			ItemViewHandle view = renderer.InternalCreate(state, model, context);
			if (!view.IsValid()) {
				renderer.Destroy(view, context);
				return null;
			}

			this.rendered[handle] = new RenderedItem(stack.GetItem(), state, view, context);
			return view;
		}

		public void Update(ItemRenderHandle handle) {
			if (!this.rendered.TryGetValue(handle, out RenderedItem entry)) return;
			this.GetRenderer(entry.item).InternalUpdate(entry.state, entry.view, entry.context);
		}

		public void Destroy(ItemRenderHandle handle, ItemRenderContext context) {
			if (!this.rendered.Remove(handle, out RenderedItem entry)) return;
			this.GetRenderer(entry.item).Destroy(entry.view, context);
		}

		public ItemRenderer GetRenderer(Item item) {
			return this.renderers[item];
		}

		public ItemModel GetModel(ItemStack stack) {
			return this.modelResolverFactory(stack.GetItem()).Resolve(stack);
		}

		internal sealed record RenderedItem(Item item, object state, ItemViewHandle view, ItemRenderContext context);
	}
}
