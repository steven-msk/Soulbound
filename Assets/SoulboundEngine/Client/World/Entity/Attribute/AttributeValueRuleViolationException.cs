using System;
using System.Runtime.Serialization;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {

	[Serializable]
	public class AttributeValueRuleViolationException : Exception {
		public AttributeValueRuleViolationException() { }
		public AttributeValueRuleViolationException(string message) : base(message) { }
		public AttributeValueRuleViolationException(string message, Exception inner) : base(message, inner) { }
		protected AttributeValueRuleViolationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
