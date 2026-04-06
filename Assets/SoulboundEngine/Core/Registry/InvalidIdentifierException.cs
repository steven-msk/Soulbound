using System;

namespace SoulboundEngine.Core.Registry {
	[Serializable]
	public class InvalidIdentifierException : Exception{
		public InvalidIdentifierException(string id)
			: base($"Invalid identifier: {id}"){
		}
	}
}
