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
			return asset.Instantiate();
		}
	}
}
