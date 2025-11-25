using System;

namespace SoulboundBackend.Client.Input {
	[Obsolete]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false)]
	public class InputActionAttribute : System.Attribute {
		public string Context { get; }
		public int Priority { get; set; }
		public string[] BlocksContexts { get; set; } = Array.Empty<string>();

		public InputActionAttribute(string context) {
			Context = context;
		}
	}
}
