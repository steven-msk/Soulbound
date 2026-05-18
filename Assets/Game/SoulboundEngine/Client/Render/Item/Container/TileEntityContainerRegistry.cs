using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item.Container {
	public static class TileEntityContainerRegistry {
		private static readonly Dictionary<TileEntityType, VisualTreeAsset> ASSETS = new();

		static TileEntityContainerRegistry() {
			Register(TileEntityTypes.CHEST, ResolveAsset("ChestContainer"));
		}

		// needs to be generic on TileEntityType
		// generic type must be container entity compatible
		// TODO: make tile entity container registration generic
		public static void Register(TileEntityType key, VisualTreeAsset asset) {
			ASSETS.Add(key, asset);
		}

		public static Func<TileEntityType, VisualTreeAsset> GetAssetFactory(HashSet<TileEntityType> tileEntityTypes) {
			Dictionary<TileEntityType, VisualTreeAsset> mappings = new();

			foreach (var tileEntityType in tileEntityTypes) {
				if (!ASSETS.TryGetValue(tileEntityType, out VisualTreeAsset asset)) {
					Logger.LogError("Tile entity container asset not found: {}", TileEntityTypes.GetIdentifier(tileEntityType));
					continue;
				}
				if (asset == null) {
					Logger.LogError("Attempted to add 'null' tile entity container: {}, skipping", GetIdentifier(tileEntityType));
					continue;
				}
				mappings.Add(tileEntityType, asset);
			}

			return tileEntityType => {
				if (!mappings.TryGetValue(tileEntityType, out VisualTreeAsset asset)) {
					Logger.LogError("Tile entity container not found: {}", GetIdentifier(tileEntityType));
					return null;
				}
				return asset;
			};
		}

		private static VisualTreeAsset ResolveAsset(string assetKey) {
			return AssetManager.Resolve<VisualTreeAsset>(new AssetKey(assetKey));
		}

		private static Identifier GetIdentifier(TileEntityType tileEntityType) {
			return TileEntityTypes.GetIdentifier(tileEntityType);
		}
	}
}
