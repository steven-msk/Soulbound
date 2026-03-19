using SoulboundBackend.Core.Assets;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.BlockSystem {
	public class Tiles : IResourceModule {
		public static Tile air => GetTile<Tile>(new AssetKey("air"));
		public static RuleTile grass => GetTile<RuleTile>(new AssetKey("grass"));
		public static Tile stone => GetTile<Tile>(new AssetKey("stone"));
		public static Tile dirt => GetTile<Tile>(new AssetKey("dirt"));
		public static Tile wood => GetTile<Tile>(new AssetKey("wood"));

		private static TTile GetTile<TTile>(AssetKey assetKey) where TTile : TileBase {
			return (TTile)IResourceModule.Resource<TileBase>(assetKey);
		}
	}
}
