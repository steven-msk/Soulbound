using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Level {
	public struct WorldSession {
		public WorldSave save;
		public LevelManager levelManager;
		public Level level;
		[Obsolete] public Canvas canvas;
		public UIDocument uiDocument;
		public Tilemap tilemap;
	}
}
