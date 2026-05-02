using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Render.Sprite;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Item {
	using Item = ItemSystem.Item;

	public static class ItemRenderers {
		private static readonly AssetKey ITEM_SPRITE_ATLAS = new("Items");
		private static readonly Dictionary<Item, IItemModelResolver.Factory> MODEL_RESOLVER_FACTORIES = new();
		private static readonly Dictionary<Item, ItemRenderer.Factory> RENDERER_FACTORIES = new();

		public static void Register(Item item, IItemModelResolver.Factory modelResolverFactory) {
			Register(item, modelResolverFactory, GetDefaultRenderer);
		}

		public static void Register(Item item, IItemModelResolver.Factory modelResolverFactory, ItemRenderer.Factory factory) {
			MODEL_RESOLVER_FACTORIES.Add(item, modelResolverFactory);
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

		public static Func<Item, IItemModelResolver> GetModelResolverFactory(ISpriteResolver<AtlasSpriteRef> spriteResolver) {
			return item => MODEL_RESOLVER_FACTORIES.TryGetValue(item, out IItemModelResolver.Factory resolverFactory)
				? resolverFactory(spriteResolver) 
				: new IItemModelResolver.Default(spriteResolver, new AtlasSpriteRef(ITEM_SPRITE_ATLAS, "missingItem"));
		}

		private static ItemRenderer GetDefaultRenderer() {
			return new ItemRenderer.Default();
		}
	}
}
