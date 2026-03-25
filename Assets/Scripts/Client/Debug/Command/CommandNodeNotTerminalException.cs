using System;

namespace SoulboundBackend.Client.Debug.Commands {
	public sealed class CommandNodeNotTerminalException : Exception {
		public CommandNodeNotTerminalException(string[] tokens, int tokenIndex)
			: base($"Node not terminal: {tokens[tokenIndex]}: " +
				  $"{CommandFormat.FormatWhere(tokens, tokenIndex)}") {
		}
	}
}
