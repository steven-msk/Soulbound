namespace SoulboundEngine.Client.World {
	using System;
	using UnityEngine;
	using UnityEngine.Tilemaps;
	using UnityEngine.UIElements;

	public interface IWorldSceneRoot {
		Grid grid { get; }
		Tilemap tilemap { get; }
		[Obsolete] Canvas canvas { get; }
		UIDocument UIDocument { get; }
	}
}
