using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public interface IEntitySubsystem {
		public void AddEntity(Entity entity);
		public void RemoveEntity(Entity entity);
	}
}
