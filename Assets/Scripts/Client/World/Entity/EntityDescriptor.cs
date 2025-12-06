using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public abstract class EntityDescriptor {
		public string name { get; private set; }
		public string ID { get; private set; }

		public EntityDescriptor(string id, string name) {
			this.name = name;
			this.ID = id;
		}

		public abstract Entity CreateInstance();
	}
}
