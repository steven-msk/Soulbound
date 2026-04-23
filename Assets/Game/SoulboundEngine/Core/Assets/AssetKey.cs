using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Core.Assets {
	public sealed record AssetKey(string address) {
		public override string ToString() => address;
	}
}
