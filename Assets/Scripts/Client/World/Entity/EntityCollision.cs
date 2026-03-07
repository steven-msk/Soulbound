using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World.EntitySystem {
	public struct EntityCollision {
		public Entity self;
		public Entity? other;
		public Vector2 point;
		public Vector2 normal;
		public GameObject otherObject;
	}
}
