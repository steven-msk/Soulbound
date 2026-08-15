using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.World.Entity {
	public interface IEntityManager {
		void AddEntity(Entity entity);
		void RemoveEntity(Entity entity);
		bool TryGetEntity(Guid guid, out Entity entity);
		IEnumerable<Entity> GetAllEntities();
	}
}
