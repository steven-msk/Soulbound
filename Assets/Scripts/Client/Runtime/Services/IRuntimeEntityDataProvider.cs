using SoulboundBackend.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Runtime.Services {
	public interface IRuntimeEntityDataProvider {
		bool TryGetEntity(Guid guid, out IEntityView entity);
		IEnumerable<IEntityView> GetAllEntities();
	}
}
