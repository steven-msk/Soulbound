using System;

namespace SoulboundEngine.Client.UI.UXMLBindings {
	[Serializable]
	public class UXMLBindingException : Exception {
		public UXMLBindingException() { }
		public UXMLBindingException(string message) : base(message) { }
		public UXMLBindingException(string message, Exception inner) : base(message, inner) { }
		protected UXMLBindingException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
