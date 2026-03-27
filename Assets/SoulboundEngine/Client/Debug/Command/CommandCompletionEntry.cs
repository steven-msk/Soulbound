using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Debug.Commands {
	public struct CommandCompletionEntry {
		public string value;
		public int length;
		public int start;

		public CommandCompletionEntry(string value, string partialToken) {
			this.value = value;
			this.length = value.Length;
			this.start = partialToken.Length;
		}
	}
}
