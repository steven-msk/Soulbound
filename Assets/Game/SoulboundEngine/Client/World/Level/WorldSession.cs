using SoulboundEngine.Client.World.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Level {
	using Player = Player.Player;

	public struct WorldSession {
		public WorldDump? deserializationData;
		public Player player;
		public LevelManager levelManager;
		public Level level;
		public Canvas canvas;
		public UIDocument uiDocument;
	}
}
