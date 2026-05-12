using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Render.Sprite;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Item {
	using Item = ItemSystem.Item;

	public static class ItemRenderers {
		private static readonly AssetKey ITEM_SPRITE_ATLAS = new("Items");
		private static readonly Dictionary<Item, IItemModelResolver.Factory> MODEL_RESOLVER_FACTORIES = new();
		private static readonly Dictionary<Item, ItemRenderer.Factory> RENDERER_FACTORIES = new();
		public static readonly Vector2 TILE_SIZE = new(8f, 8f);
		public static readonly Vector2 DEFAULT_SPRITE_SIZE = new(32f, 32f);

		static ItemRenderers() {
			Register(Items.GRASS, DefaultResolverFactory("grass_top", TILE_SIZE));
			Register(Items.DIRT, DefaultResolverFactory("dirt", TILE_SIZE));
			Register(Items.STONE, DefaultResolverFactory("stone", TILE_SIZE));
			Register(Items.WOOD, DefaultResolverFactory("wood", TILE_SIZE));
			Register(Items.LEAVES, DefaultResolverFactory("leaves", TILE_SIZE));

			Register(Items.placeableItem, DefaultResolverFactory("bluething", DEFAULT_SPRITE_SIZE));
			Register(Items.teleportPlayerItem, DefaultResolverFactory("bluething", DEFAULT_SPRITE_SIZE));
			Register(Items.spawnEntityItem, DefaultResolverFactory("bluething", DEFAULT_SPRITE_SIZE));
			Register(Items.chargeableItem, DefaultResolverFactory("bluething", DEFAULT_SPRITE_SIZE));
			Register(Items.debugPointer, DefaultResolverFactory("debugPointer", DEFAULT_SPRITE_SIZE));
			Register(Items.inventoryListenerItem, DefaultResolverFactory("bluething", DEFAULT_SPRITE_SIZE));
			Register(Items.blockBreakerItem, DefaultResolverFactory("bluething", DEFAULT_SPRITE_SIZE));
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
				: DefaultResolverFactory("missingItem", new Vector2(32f, 32f))(spriteResolver);
		}

		private static IItemModelResolver.Factory DefaultResolverFactory(string spriteKey, Vector2 referenceSize) {
			AtlasSpriteRef spriteRef = new(ITEM_SPRITE_ATLAS, spriteKey);
			return spriteResolver => new IItemModelResolver.Default(spriteResolver, spriteRef, referenceSize);
		}

		private static ItemRenderer GetDefaultRenderer() {
			return new ItemRenderer.Default();
		}
	}
}
