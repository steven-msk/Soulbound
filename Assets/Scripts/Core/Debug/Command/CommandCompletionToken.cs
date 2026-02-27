using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public struct CommandCompletionToken {
		public string text;
		public int absoluteStart;
		public int replaceLength;
	}
}
