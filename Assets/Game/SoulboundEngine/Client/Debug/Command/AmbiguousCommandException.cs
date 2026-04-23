using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Debug.Commands {

	[Serializable]
	public class AmbiguousCommandException : Exception {
		public AmbiguousCommandException(string parent, string child, string sibling, IEnumerable<string> inputs)
			: base($"Ambiguous command node between '{child}' and '{sibling}' in parent node '{parent}'. " +
				  $"Inputs: [{string.Join(',', inputs)}]") {
		}
	}
}
