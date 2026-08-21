namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World.Serialization;
	using System;
	using UnityEngine;
	using UnityEngine.Tilemaps;
	using UnityEngine.UIElements;

	public struct WorldSession {
		public WorldSave save;
		public LevelManager levelManager;
		public Level level;
		[Obsolete] public Canvas canvas;
		public UIDocument uiDocument;
		public Tilemap tilemap;
	}
}
