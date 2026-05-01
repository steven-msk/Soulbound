using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Item {
	using Item = ItemSystem.Item;

	public static class ItemRenderers {
		private static readonly Dictionary<Item, IItemModelResolver> MODEL_RESOLVERS = new();
		private static readonly Dictionary<Item, ItemRenderer.Factory> RENDERER_FACTORIES = new();

		public static void Register<R>(Item item, R modelResolver) where R : IItemModelResolver {
			Register(item, modelResolver, GetDefaultRenderer);
		}

		public static void Register<R>(Item item, R modelResolver, ItemRenderer.Factory factory) where R : IItemModelResolver {
			MODEL_RESOLVERS.Add(item, modelResolver);
			RENDERER_FACTORIES.Add(item, factory);
		}

		public static Dictionary<Item, ItemRenderer> LoadRenderers(List<Item> items) {
			Dictionary<Item, ItemRenderer> rendererByItem = new();
			foreach (var item in items) {
				rendererByItem.Add(item, RENDERER_FACTORIES.TryGetValue(item, out ItemRenderer.Factory factory)
					? factory()
					: GetDefaultRenderer()
				);	
			}
			return rendererByItem;
		}

		public static Func<Item, IItemModelResolver> GetModelResolverFactory() {
			return item => MODEL_RESOLVERS[item];
		}

		private static ItemRenderer GetDefaultRenderer() {
			return new ItemRenderer.Default();
		}
	}
}
