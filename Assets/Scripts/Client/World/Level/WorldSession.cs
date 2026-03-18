using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World {
	public struct WorldSession {
		public WorldDump? deserializationData;
		public Player player;
		public LevelManager levelManager;
		public Level level;
		public Canvas canvas;
	}
}
