using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	public class EntityDescriptor {
		private readonly string id;
		private readonly Func<Vector2, Entity> factory;

		public EntityDescriptor(string id, Func<Vector2, Entity> factory) {
			this.id = id;
			this.factory = factory;
		}

		public string GetID() => id;

		public Entity Create(Vector2 pos) => factory(pos);

		public override string ToString() => id;

		public readonly struct RegistrationKey : IRegistrationKey<EntityDescriptor> {
			public readonly string descriptorID;

			public RegistrationKey(string descriptorID) {
				this.descriptorID = descriptorID;
			}

			public override bool Equals(object obj) {
				return obj is RegistrationKey descriptorKey
					&& this.descriptorID == descriptorKey.descriptorID;
			}

			public override int GetHashCode() => HashCode.Combine(descriptorID);
		}
	}
}
