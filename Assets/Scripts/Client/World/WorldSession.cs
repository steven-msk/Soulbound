using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World {
	public struct WorldSession {
		public WorldDump? deserializationData;
		public PlayerController player;
		public LevelManager levelManager;
		public EntityManager entityManager;
		public Level level;
	}
}
