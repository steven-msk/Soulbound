using SoulboundEngine.Core.Render.Sprite;
using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class WorldItemRenderer {
		private readonly ISpriteResolver<SpriteRef> spriteResolver;

		public WorldItemRenderer(ISpriteResolver<SpriteRef> spriteResolver) {
			this.spriteResolver = spriteResolver;
		}

		public WorldItemView CreateView(GameObject obj) {
			WorldItemView view = obj.AddComponent<WorldItemView>();

			SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
			view.Init(spriteRenderer);

			obj.SetActive(false);
			return view;
		}

		public void Render(WorldItemView view, ItemRenderModel model) {
			SpriteRenderer spriteRenderer = view.GetSpriteRenderer();
			Sprite sprite = spriteResolver.GetSprite(model.spriteRef);
			spriteRenderer.sprite = sprite;

			view.gameObject.SetActive(true);
		}
	}
}
