using UnityEngine;

#nullable enable

namespace SoulboundEngine.Common.Unity {
	public static class ComponentUtility {
		public static TComponent? GetComponent<TComponent>(this GameObject gameObject) where TComponent : Component {
			return gameObject.GetComponent<TComponent>();
		}

		public static TComponent? AddComponent<TComponent>(this GameObject gameObject) where TComponent : Component {
			return gameObject.AddComponent<TComponent>();
		}

		public static TComponent? GetOrAddComponent<TComponent>(this GameObject gameObject) where TComponent : Component {
			var component = GetComponent<TComponent>(gameObject) ?? AddComponent<TComponent>(gameObject);
			return component;
		}
	 }
}
