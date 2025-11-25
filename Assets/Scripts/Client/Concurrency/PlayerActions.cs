using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public static class PlayerActions {
		public static readonly ActionToken ItemUse = new();
		public static readonly ActionToken BlockBreak = new();
		public static readonly ActionToken Attack = new();
	}
}
