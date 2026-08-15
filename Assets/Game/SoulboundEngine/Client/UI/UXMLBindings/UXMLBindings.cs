using SoulboundEngine.Registry;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.UXMLBindings {
	public static class UXMLBindings {
		public static T Get<T>(this VisualElement root, Identifier id) where T : VisualElement {
			Type expected = UXMLSchema.Resolve(id);
			if (!typeof(T).IsAssignableFrom(expected)) {
				throw new UXMLBindingException($"'{id}' is {expected.Name}, requested {typeof(T).Name}.");
			}
			string elementId = id.SplitPath().Last();
			T element = root.Q<T>(elementId);
			return element ?? throw new UXMLBindingException($"'{id}' not found under given root: asset/schema mismatch.");
		}
	}
}
