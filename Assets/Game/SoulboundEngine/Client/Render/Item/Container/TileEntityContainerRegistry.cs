using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item.Container {
	public static class TileEntityContainerRegistry {
		private static Dictionary<RegistryKey<TileEntityType>, VisualTreeAsset> ASSETS = new();

		public static void Register(RegistryKey<TileEntityType> key, VisualTreeAsset asset) {
			ASSETS.Add(key, asset);
		}

	}
}
