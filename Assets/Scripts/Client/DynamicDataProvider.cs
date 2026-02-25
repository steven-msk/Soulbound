using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client {
	public sealed class DynamicDataProvider : IDynamicDataProvider {
		public IDynamicPlayerDataProvider Player => throw new NotImplementedException();

		public IDynamicEntityDataProvider Entities => throw new NotImplementedException();
	}
}
