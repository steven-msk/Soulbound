using SoulboundBackend.Client.World.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Combat {
	public struct AttackContext {
		public Entity performer;
		public AttackSource source;
		public object metadata;

		public AttackContext(Entity performer, AttackSource source) {
			this.performer = performer;
			this.source = source;
			this.metadata = null;
		}
	}
}
