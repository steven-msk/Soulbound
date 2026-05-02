using SoulboundEngine.Client.ItemSystem;
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

		static ItemRenderers() {
			Register(Items.GRASS, DefaultResolverFactory("grass_top"));
			Register(Items.DIRT, DefaultResolverFactory("dirt"));
			Register(Items.STONE, DefaultResolverFactory("stone"));
			Register(Items.WOOD, DefaultResolverFactory("wood"));
			Register(Items.LEAVES, DefaultResolverFactory("leaves"));

			Register(Items.placeableItem, DefaultResolverFactory("bluething"));
			Register(Items.teleportPlayerItem, DefaultResolverFactory("bluething"));
			Register(Items.spawnEntityItem, DefaultResolverFactory("bluething"));
			Register(Items.chargeableItem, DefaultResolverFactory("bluething"));
			Register(Items.debugPointer, DefaultResolverFactory("debugPointer"));
			Register(Items.inventoryListenerItem, DefaultResolverFactory("bluething"));
			Register(Items.blockBreakerItem, DefaultResolverFactory("bluething"));
		}

		public static void Register(Item item, IItemModelResolver.Factory modelResolverFactory) {
			Register(item, modelResolverFactory, GetDefaultRenderer);
		}

		public static void Register(Item item, IItemModelResolver.Factory modelResolverFactory, ItemRenderer.Factory rendererFactory) {
			MODEL_RESOLVER_FACTORIES.Add(item, modelResolverFactory);
			RENDERER_FACTORIES.Add(item, rendererFactory);
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
				: DefaultResolverFactory("missingItem")(spriteResolver);
		}

		private static IItemModelResolver.Factory DefaultResolverFactory(string spriteKey) {
			AtlasSpriteRef spriteRef = new(ITEM_SPRITE_ATLAS, spriteKey);
			return spriteResolver => new IItemModelResolver.Default(spriteResolver, spriteRef);
		}

		private static ItemRenderer GetDefaultRenderer() {
			return new ItemRenderer.Default();
		}
	}
}
