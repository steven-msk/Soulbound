using UnityEngine;

#nullable enable

public static class ComponentUtility {
	public static TComponent? GetComponent<TComponent>(this GameObject gameObject) where TComponent : Component {
		return gameObject.GetComponent<TComponent>();
	}

	public static TComponent? AddComponent<TComponent>(this GameObject gameObject) where TComponent : Component {
		return gameObject.AddComponent<TComponent>();
	}

	public static TComponent? GetOrAddComponent<TComponent>(this GameObject gameObject) where TComponent : Component {
		var component = ComponentUtility.GetComponent<TComponent>(gameObject);
		if (component == null) {
			component = ComponentUtility.AddComponent<TComponent>(gameObject);
		}
		return component;
	}
 }
