using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.TileDefinitions {
	[CreateAssetMenu(menuName = "Tiles/AirEdgeTile")]
	public class GrassTile : RuleTile<GrassTile.Neighbor> {
		public class Neighbor : RuleTile.TilingRule.Neighbor {
			public const int IsAir = 3;
			public const int ThisOrDirt = 4;
		}

		// FIXME: visual grass tile bug
		// Sometimes the tiles dont update properly and a grass tile sets itself to the wrong state
		// This can be fixed manually by updating the position with a tile brush,
		// but this is not maintainable and works only in editor

		public override bool RuleMatch(int neighbor, TileBase other) {
			return neighbor switch {
				Neighbor.This => other == this,
				Neighbor.NotThis => other != this,
				Neighbor.IsAir => other == null || other == Tiles.air,
				Neighbor.ThisOrDirt => other != null && (other == this || other == Tiles.dirt),
				_ => base.RuleMatch(neighbor, other)
			};
		}
	}
}
