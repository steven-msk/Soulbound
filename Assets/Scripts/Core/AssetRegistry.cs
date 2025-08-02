using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable

public static class AssetRegistry {
	public static Dictionary<string, Object> resources = new();

	public static void Reset() => resources.Clear();

	public static T Register<T>(T resource) where T : Object {
		string ID = resource is ISerializable serializable ? serializable.ID : resource.name;
		if (resources.ContainsKey(ID)) {
			Debug.LogWarning($"Skipping registration of duplicate registry ID {ID} of type {typeof(T)}");
			return resource;
		}
		resources[ID] = resource;
		return resource;
	}

	public static T? Get<T>(string ID) where T : Object {
		if (resources.ContainsKey(ID)) {
			return (T)resources[ID];
		}
		Debug.LogError($"Could not find resource '{ID}' in registry"); 
		return (T)resources.GetValueOrDefault(ID, null);
	}

	public static List<T> GetAll<T>() where T : Object => GameManager.instance.GetAll<T>();

    // FIXME: different types for the same ID causes issues with registration

    public static List<T> RegisterAll<T>(string path) where T : Object {
		Object[] resources = Resources.LoadAll<T>(path);
		if (resources.Length == 0) {
			Debug.LogWarning($"No '{typeof(T).Name}' resources found at 'Resources/{path}'. Check path and asset types.");
			return new List<T>();
		}
		List<T> addedResources = new();
        foreach (var resource in resources) {
			if (AssetRegistry.resources.TryGetValue(resource.name, out var resourceTest) && resourceTest.GetType() == typeof(T)) {
				Debug.LogWarning($"Duplicate resource found: {resource.name} at path '{path}'. Skipping registration.");
				continue;
			}
            try {
				InvocationHelper.PatternIfElse<ISerializable>(resource, 
					(serializable) => AssetRegistry.resources.Add(serializable.ID, resource), 
					() => AssetRegistry.resources.Add(resource.name, resource));
				addedResources.Add((T)resource);
            } catch (Exception e) {
				Debug.LogError($"Failed to register resource '{resource.name}' at path '{path}': {e.Message}");
            }
        }
		return addedResources;
	}
}
