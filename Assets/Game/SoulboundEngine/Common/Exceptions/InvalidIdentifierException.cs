using System;

namespace SoulboundEngine.Core.Registry {
	/// <summary>
	/// Throws when parsing or constructing an `Identifier` that contains an invalid character.
	/// <b>This should not be caught.</b>
	/// </summary>
	[Serializable]
	public class InvalidIdentifierException : Exception {
		public InvalidIdentifierException(string id)
			: base($"Invalid identifier: {id}") {
		}
	}
}
