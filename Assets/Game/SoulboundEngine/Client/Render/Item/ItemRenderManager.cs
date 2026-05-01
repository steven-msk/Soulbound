using SoulboundEngine.Client.ItemSystem;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Item {
	using Item = ItemSystem.Item;

	public sealed class ItemRenderManager {
		private readonly Dictionary<Item, ItemRenderer> renderers;
		private readonly Func<Item, IItemModelResolver> modelResolverFactory;
		private readonly Dictionary<int, RenderedItem> rendered = new();

		public ItemRenderManager(List<Item> items) {
			this.modelResolverFactory = ItemRenderers.GetModelResolverFactory();
			this.renderers = ItemRenderers.LoadRenderers(items);
		}

		public void Render(int key, ItemStack stack, ItemRenderContext context) {
			if (this.rendered.ContainsKey(key)) {
				this.Destroy(key);
			}

			ItemRenderer renderer = this.renderers[stack.item];
			IItemModelResolver modelResolver = this.modelResolverFactory(stack.item);

			ItemModel model = modelResolver.Resolve(stack);
			object state = renderer.CreateRenderStateBoxed(stack, model);
			IItemView view = renderer.CreateViewBoxed(state, context);

			this.rendered[key] = new RenderedItem(stack.item, state, view);
		}

		public void Update(int key, ItemRenderContext context) {
			if (!this.rendered.TryGetValue(key, out RenderedItem entry)) return;

			this.GetRenderer(entry).UpdateViewBoxed(entry.state, entry.view, context);
		}

		public void Destroy(int key) {
			if (!this.rendered.Remove(key, out RenderedItem entry)) return;
			this.GetRenderer(entry).DestroyView(entry.view);
		}

		private ItemRenderer GetRenderer(RenderedItem renderedItem) {
			return this.renderers[renderedItem.item];
		}

		internal sealed record RenderedItem(Item item, object state, IItemView view);
	}
}
