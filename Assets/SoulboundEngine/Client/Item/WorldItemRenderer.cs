using SoulboundEngine.Client.ItemSystem.View;
using SoulboundEngine.Core.Assets;
using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class WorldItemRenderer {
		public WorldItemView CreateView(GameObject obj) {
			WorldItemView view = obj.AddComponent<WorldItemView>();

			SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
			view.Init(spriteRenderer);

			obj.SetActive(false);
			return view;
		}

		public void Render(WorldItemView view, ItemRenderModel model) {
			SpriteRenderer spriteRenderer = view.GetSpriteRenderer();
			Sprite sprite = AssetManager.Resolve<Sprite>(model.spriteKey);
			spriteRenderer.sprite = sprite;

			view.gameObject.SetActive(true);
		}
	}
}
