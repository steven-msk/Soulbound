using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Debug.Commands {
	public struct CommandCompletionToken {
		public string value;
		public int start;
		public int length;

		public CommandCompletionToken(string value, string partialToken) {
			this.value = value;
			this.length = value.Length;
			this.start = partialToken.Length;
		}
	}
}
