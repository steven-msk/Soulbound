using SoulboundEngine.Client.World;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace SoulboundEngine.World.Level {
	public struct WorldSession {
		public WorldSave save;
		public LevelManager levelManager;
		public Level level;
		[Obsolete] public Canvas canvas;
		public UIDocument uiDocument;
		public Tilemap tilemap;
	}
}
