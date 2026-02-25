using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client {
	public interface IDynamicDataProvider {
		IDynamicPlayerDataProvider Player { get; }
		IDynamicEntityDataProvider Entities { get; }
	}
}
