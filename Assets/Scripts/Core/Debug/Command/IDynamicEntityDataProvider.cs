using Assets.Scripts.Core.Debug.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public interface IDynamicEntityDataProvider {
		bool TryGetEntity(Guid guid, out IEntityView entity);
		IEnumerable<IEntityView> GetAllEntities();
	}
}
