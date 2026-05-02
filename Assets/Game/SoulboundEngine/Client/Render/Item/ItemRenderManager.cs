using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core.Render.Sprite;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Item {
	using Item = ItemSystem.Item;

	public sealed class ItemRenderManager {
		private readonly Dictionary<Item, ItemRenderer> renderers;
		private readonly Func<Item, IItemModelResolver> modelResolverFactory;
		private readonly Dictionary<RenderHandle, RenderedItem> rendered = new();

		public ItemRenderManager(List<Item> items, ISpriteResolver<AtlasSpriteRef> spriteResolver) {
			this.modelResolverFactory = ItemRenderers.GetModelResolverFactory(spriteResolver);
			this.renderers = ItemRenderers.LoadRenderers(items);
		}

		public IItemView Render(RenderHandle handle, ItemStack stack, ItemRenderContext context) {
			if (this.rendered.ContainsKey(handle)) {
				this.Destroy(handle);
			}

			ItemRenderer renderer = this.renderers[stack.item];
			IItemModelResolver modelResolver = this.modelResolverFactory(stack.item);

			ItemModel model = modelResolver.Resolve(stack);
			object state = renderer.CreateRenderStateBoxed(stack, model);
			IItemView view = renderer.CreateViewBoxed(state, context);

			this.rendered[handle] = new RenderedItem(stack.item, state, view, context);
			return view;
		}

		public void Update(RenderHandle handle) {
			if (!this.rendered.TryGetValue(handle, out RenderedItem entry)) return;

			this.GetRenderer(entry).UpdateViewBoxed(entry.state, entry.view, entry.context);
		}

		public void Destroy(RenderHandle handle) {
			if (!this.rendered.Remove(handle, out RenderedItem entry)) return;
			this.GetRenderer(entry).DestroyView(entry.view);
		}

		private ItemRenderer GetRenderer(RenderedItem renderedItem) {
			return this.renderers[renderedItem.item];
		}

		internal sealed record RenderedItem(Item item, object state, IItemView view, ItemRenderContext context);
	}
}
