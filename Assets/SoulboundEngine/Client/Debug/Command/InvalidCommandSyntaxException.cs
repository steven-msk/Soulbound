using System;

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed class InvalidCommandSyntaxException : Exception {
		public InvalidCommandSyntaxException(string message, string[] tokens, int tokenIndex, string format = CommandFormat.MARKER_FORMAT)
			: base($"{message}: {CommandFormat.FormatWhere(tokens, tokenIndex, format)}") {
		}

		public InvalidCommandSyntaxException(string message)
			: base(message) {
		}
	}
}
