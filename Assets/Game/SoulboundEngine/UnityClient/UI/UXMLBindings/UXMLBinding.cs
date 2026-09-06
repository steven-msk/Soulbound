namespace SoulboundEngine.UnityClient.UI.UXMLBindings {
	using SoulboundEngine.Registry;
	using System;
	using System.Linq;
	using UnityEngine.UIElements;

	public class UXMLBinding<T> where T : VisualElement {
		private readonly Identifier identifier;

		public UXMLBinding(string id) 
			: this(Identifier.Of(id)) {
		}

		public UXMLBinding(Identifier identifier) {
			this.identifier = identifier;
		}

		public T Get(VisualElement root) {
			Type expected = UXMLSchema.Resolve(this.identifier);
			if (!typeof(T).IsAssignableFrom(expected)) {
				throw new UXMLBindingException($"'{this.identifier}' is {expected.Name}, requested {typeof(T).Name}.");
			}

			string elementId = this.identifier.SplitPath().Last();
			T element = root.Q<T>(elementId);
			return element ?? throw new UXMLBindingException($"'{this.identifier}' not found under given root: " + root.name);
		}

		public Identifier GetIdentifier() => this.identifier;
	}
}
