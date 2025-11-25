using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public static class PlayerActionTokens {
		public static readonly ActionToken ItemUse = new(1);
		public static readonly ActionToken BlockBreak = new(2);
		public static readonly ActionToken Attack = new(3);
	}
}
