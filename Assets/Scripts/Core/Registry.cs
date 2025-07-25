using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;

public static class Registry {
	static Dictionary<string, Object> resources = new();

	public static void Reset() => resources = new();

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

	public static void RegisterAll<T>(string path) where T : Object {
		Object[] resources = Resources.LoadAll<T>(path);
		if (resources.Length == 0) {
			Debug.LogWarning($"No '{typeof(T).Name}' resources found at 'Resources/{path}'. Check path and asset types.");
			return;
		}
		Registry.resources.AddRange(resources.Where(resource => resource is ISerializable)
			.Select(resource => new KeyValuePair<string, Object>(((ISerializable)resource).ID, resource)));
		Registry.resources.AddRange(resources.Where(resource => resource is not ISerializable)
			.Select(resource => new KeyValuePair<string, Object>(resource.name, resource)));
	}
}
