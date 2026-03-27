using System;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class UnknownOrIncompleteCommandException : Exception {
		public UnknownOrIncompleteCommandException(string[] tokens, int tokenIndex)
			: base($"Unknown or incomplete command at token '{tokens[tokenIndex]}': " +
				  $"{CommandFormat.FormatWhere(tokens, tokenIndex)}") {
		}
	}
}
