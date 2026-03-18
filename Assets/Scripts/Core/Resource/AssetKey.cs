using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.AssetManagement {
	public sealed record AssetKey(string address) {
		public override string ToString() => address;
	}
}
