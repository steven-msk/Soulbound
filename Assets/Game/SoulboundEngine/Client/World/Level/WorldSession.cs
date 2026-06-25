using SoulboundEngine.Client.World.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.Level {
	using PlayerEntity = Player.PlayerEntity;

	public struct WorldSession {
		public WorldDump? deserializationData;
		public PlayerEntity player;
		public LevelManager levelManager;
		public Level level;
		public Canvas canvas;
		public UIDocument uiDocument;
	}
}
