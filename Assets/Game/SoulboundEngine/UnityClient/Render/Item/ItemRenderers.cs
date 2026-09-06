namespace SoulboundEngine.UnityClient.Render.Item {
	using SoulboundEngine.Item;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Render.Sprite;
	using System;
	using System.Collections.Generic;
	using Item = SoulboundEngine.Item.Item;

	public static class ItemRenderers {
		private static readonly AssetKey ITEM_SPRITE_ATLAS = new("Items");
		private const string MISSING = "missingItem";
		private static readonly Dictionary<Item, IItemModelResolver.Factory> MODEL_RESOLVER_FACTORIES = new();
		private static readonly Dictionary<Item, ItemRenderer.Factory> RENDERER_FACTORIES = new();

		static ItemRenderers() {
			Register(Items.AIR, DefaultResolverFactory("air"), () => new AirItemRenderer());
			Register(Items.GRASS, DefaultResolverFactory("grass_top"));
			Register(Items.DIRT, DefaultResolverFactory("dirt"));
			Register(Items.STONE, DefaultResolverFactory("stone"));
			Register(Items.WOOD, DefaultResolverFactory("wood"));
			Register(Items.LEAVES, DefaultResolverFactory("leaves"));
			Register(Items.CHEST, DefaultResolverFactory("chest"));
			Register(Items.WOODEN_PICKAXE, DefaultResolverFactory("wooden_pickaxe"));
			Register(Items.STONE_PICKAXE, DefaultResolverFactory("stone_pickaxe"));
			Register(Items.WOODEN_BOOTS, DefaultResolverFactory("wooden_boots"));
			Register(Items.WOODEN_LEGGINGS, DefaultResolverFactory("wooden_leggings"));
			Register(Items.WOODEN_CHESTPLATE, DefaultResolverFactory("wooden_chestplate"));
			Register(Items.WOODEN_HELMET, DefaultResolverFactory("wooden_helmet"));
			Register(Items.STONE_BOOTS, DefaultResolverFactory("stone_boots"));
			Register(Items.STONE_LEGGINGS, DefaultResolverFactory("stone_leggings"));
			Register(Items.STONE_CHESTPLATE, DefaultResolverFactory("stone_chestplate"));
			Register(Items.STONE_HELMET, DefaultResolverFactory("stone_helmet"));

			Register(Items.placeableItem, DefaultResolverFactory("bluething"));
			Register(Items.teleportPlayerItem, DefaultResolverFactory("bluething"));
			Register(Items.debugPointer, DefaultResolverFactory("debugPointer"));
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
			foreach (Item item in items) {
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
				: DefaultResolverFactory(MISSING)(spriteResolver);
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
