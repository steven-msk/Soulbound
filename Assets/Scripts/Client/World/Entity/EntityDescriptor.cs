using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public class EntityDescriptor {
		public readonly string id;
		private readonly Func<Vector2, Entity> factory;

		public EntityDescriptor(string id, Func<Vector2, Entity> factory) {
			this.id = id;
			this.factory = factory;
		}

		public Entity Create(Vector2 pos) => factory(pos);

		public override string ToString() => id;
	}
}
