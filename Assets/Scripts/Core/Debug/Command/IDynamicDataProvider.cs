using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public interface IDynamicDataProvider {
		IDynamicPlayerDataProvider Player { get; }
		IDynamicEntityDataProvider Entities { get; }
	}
}
