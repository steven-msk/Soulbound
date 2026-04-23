using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.World.Serialization;
using UnityEngine;

namespace SoulboundEngine.Client.World.LevelDomain {
	public struct WorldSession {
		public WorldDump? deserializationData;
		public Player player;
		public LevelManager levelManager;
		public Level level;
		public Canvas canvas;
	}
}
