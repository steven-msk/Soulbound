using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Registry {
	public static Dictionary<string, Object> resources = new();

	public static void Reset() => resources.Clear();

	public static T Register<T>(T resource) where T : Object {
		string ID = resource is ISerializable serializable ? serializable.ID : resource.name;
		if (resources.ContainsKey(ID)) {
			Debug.LogWarning($"Duplicate item registry ID: {ID}");
			return resource;
		}
		resources[ID] = resource;
		return resource;
	}

	[CanBeNull] public static T Get<T>(string ID) where T : Object {
		if (resources.ContainsKey(ID)) {
			return (T)resources[ID];
		}
		Debug.LogError($"Could not find resource '{ID}' in registry"); 
		return (T)resources.GetValueOrDefault(ID, default);
	}

    // FIXME: different types for the same ID causes issues with registration

    public static void RegisterAll<T>(string path) where T : Object {
		Object[] resources = Resources.LoadAll<T>(path);
		if (resources.Length == 0) {
			Debug.LogWarning($"No '{typeof(T).Name}' resources found at 'Resources/{path}'. Check path and asset types.");
			return;
		}
        foreach (var resource in resources) {
			if (Registry.resources.TryGetValue(resource.name, out var resourceTest) && resourceTest.GetType() == typeof(T)) {
				Debug.LogWarning($"Duplicate resource found: {resource.name} at path '{path}'. Skipping registration.");
				continue;
			}
            try {
				InvocationHelper.PatternIfElse<ISerializable>(resource, 
					(serializable) => Registry.resources.Add(serializable.ID, resource), 
					() => Registry.resources.Add(resource.name, resource));
			} catch (ArgumentException e) {
				Debug.LogError($"Failed to register resource '{resource.name}' at path '{path}': {e.Message}");
            }
        }
	}
}
