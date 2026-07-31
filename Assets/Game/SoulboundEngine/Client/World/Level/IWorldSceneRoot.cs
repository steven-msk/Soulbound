using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Level {
	public interface IWorldSceneRoot {
		Grid grid { get; }
		Tilemap tilemap { get; }
		[Obsolete] Canvas canvas { get; }
		UIDocument UIDocument { get; }

		public LevelGridContext GetGridContext() => new(this.grid, this.tilemap);
	}
}
