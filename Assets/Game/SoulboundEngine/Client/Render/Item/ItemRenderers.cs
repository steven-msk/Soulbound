using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Render.Item {
	using Item = ItemSystem.Item;

	public static class ItemRenderers {
		private static readonly Dictionary<Item, IItemModelResolver> MODEL_RESOLVERS = new();
		private static readonly Dictionary<Item, ItemRenderer.Factory> RENDERER_FACTORIES = new();

		public static void Register<R>(Item item, R modelResolver) where R : IItemModelResolver {
			MODEL_RESOLVERS.Add(item, modelResolver);
		}

		public static Dictionary<Item, ItemRenderer> LoadRenderers() {
			return RENDERER_FACTORIES.ToDictionary(kvp => kvp.Key, kvp => kvp.Value());
		}

		public static Func<Item, IItemModelResolver> GetModelResolverFactory() {
			return item => MODEL_RESOLVERS[item];
		}
	}
}
