using SoulboundEngine.World.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.World.Services {
	public interface IRuntimeEntityDataProvider {
		bool TryGetEntity(Guid guid, out IEntityView entity);
		IEnumerable<IEntityView> GetAllEntities();
	}
}
