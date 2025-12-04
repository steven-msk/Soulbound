using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client {
	public sealed class PlayerSpawnData : ISpawnData {
		public Vector2 position { get; init; }

	}
}
