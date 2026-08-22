namespace SoulboundEngine.World.Level {
	using System;
	using UnityEngine;
	using UnityEngine.Tilemaps;
	using UnityEngine.UIElements;

	public partial struct WorldSession {
		[Obsolete] public Canvas canvas;
		public UIDocument uiDocument;
		public Tilemap tilemap;
	}
}
