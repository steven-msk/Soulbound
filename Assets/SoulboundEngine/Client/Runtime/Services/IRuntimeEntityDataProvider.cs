using SoulboundEngine.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Runtime.Services {
	public interface IRuntimeEntityDataProvider {
		bool TryGetEntity(Guid guid, out IEntityView entity);
		IEnumerable<IEntityView> GetAllEntities();
	}
}
