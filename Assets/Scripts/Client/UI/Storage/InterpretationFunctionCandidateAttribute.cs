#nullable enable

using System;

namespace SoulboundBackend.Client.UI.Storage {
	[Obsolete]
	public class InterpretationFunctionCandidateAttribute : System.Attribute {
		public string? description { get; }
		public string[] usedSubmodules { get; }

		public InterpretationFunctionCandidateAttribute(string? description = "", params string[] usedSubmodules) {
			this.description = description;
			this.usedSubmodules = usedSubmodules;
		}
	}
}
