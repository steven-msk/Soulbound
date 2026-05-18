using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item.Container {
	public class TileEntityContainerRenderer {
		private readonly Func<TileEntityType, VisualTreeAsset> assetFactory;

		public TileEntityContainerRenderer(Func<TileEntityType, VisualTreeAsset> assetFactory) {
			this.assetFactory = assetFactory;
		}

		public VisualElement Render(TileEntityType tileEntityType) {
			VisualTreeAsset asset = this.assetFactory(tileEntityType);
			if (asset == null) return null;

			TemplateContainer element = asset.CloneTree();
			element.style.position = Position.Absolute;
			element.style.top = element.style.right = element.style.bottom = element.style.left = 0;
			return element;
		}
	}
}
