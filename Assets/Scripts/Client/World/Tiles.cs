using SoulboundBackend.Core.Resource;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World {
	public class Tiles : IResourceModule {
		public static Tile air => GetTile<Tile>("air");
		public static RuleTile grass => GetTile<RuleTile>("grass");
		public static Tile stone => GetTile<Tile>("stone");
		public static Tile dirt => GetTile<Tile>("dirt");
		public static Tile wood => GetTile<Tile>("wood");

		private static TTile GetTile<TTile>(string name) where TTile : TileBase {
			return (TTile)IResourceModule.Resource<TileBase, ResourceGroups.Tiles>(name);
		}
	}
}
