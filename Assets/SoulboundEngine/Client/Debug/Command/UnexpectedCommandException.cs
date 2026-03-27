using System;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class UnexpectedCommandException : Exception {
		public UnexpectedCommandException(string[] tokens, int tokenIndex)
			: base($"Unexpected command exception at token '{tokens[tokenIndex]}': " +
				  $"{CommandFormat.FormatWhere(tokens, tokenIndex)}") {
		}
	}
}
