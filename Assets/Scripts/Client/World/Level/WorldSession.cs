using SoulboundBackend.Client.Players;
using SoulboundBackend.Client.World.Serialization;
using SoulboundBackend.Common;
using UnityEngine;

namespace SoulboundBackend.Client.World.LevelDomain {
	public struct WorldSession {
		public WorldDump? deserializationData;
		public Player player;
		public LevelManager levelManager;
		public Level level;
		public Canvas canvas;

		[PROTOTYPICAL]
		public AudioSource audioSource;
	}
}
