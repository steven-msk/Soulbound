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
			ItemModel model = this.GetModel(stack);
			object state = renderer.CreateRenderStateBoxed(stack, model, context);
			IItemView view = renderer.CreateViewBoxed(state, context);

			this.rendered[handle] = new RenderedItem(stack.item, state, view, context);
			return view;
		}

		public void Update(RenderHandle handle) {
			if (!this.rendered.TryGetValue(handle, out RenderedItem entry)) return;

			this.GetRenderer(entry.item).UpdateViewBoxed(entry.state, entry.view, entry.context);
		}

		public void Destroy(RenderHandle handle) {
			if (!this.rendered.Remove(handle, out RenderedItem entry)) return;
			this.GetRenderer(entry.item).DestroyView(entry.view);
		}

		public ItemRenderer GetRenderer(Item item) {
			return this.renderers[item];
		}

		public ItemModel GetModel(ItemStack stack) {
			return this.modelResolverFactory(stack.item).Resolve(stack);
		}

		internal sealed record RenderedItem(Item item, object state, IItemView view, ItemRenderContext context);
	}
}
