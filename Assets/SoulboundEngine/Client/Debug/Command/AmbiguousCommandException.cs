using System;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class AmbiguousCommandException : Exception {
		public AmbiguousCommandException(string[] matching, string[] tokens, int tokenIndex, string format = CommandFormat.MARKER_FORMAT)
			:  base($"Ambiguity between {CommandFormat.FormatQuoting(matching)}" +
				   $" at token '{tokens[tokenIndex]}': " +
				   $"\"{CommandFormat.FormatWhere(tokens, tokenIndex, format)}\"") {
		}
	}
}
