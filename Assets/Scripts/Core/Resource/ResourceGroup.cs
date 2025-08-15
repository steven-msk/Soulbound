using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "Resource Group")]
public class ResourceGroup : ScriptableObject {
	private static readonly Logger logger = Logger.CreateInstance();
	public string groupAddress;
	public string searchFolder = "Assets/Resources/";
	[SerializeField] private string assetType;
	public Type AssetType => string.IsNullOrEmpty(assetType) ? null : Type.GetType(assetType);
	[SerializeField] private Object[] resources;


#if UNITY_EDITOR
	[ContextMenu("Refresh Group")]
	public void RefreshGroup() {
		var guids = AssetDatabase.FindAssets($"t:{assetType}", new[] { searchFolder });

		resources = guids
			.Select(g => AssetDatabase.GUIDToAssetPath(g))
			.Where(path => System.IO.Path.GetDirectoryName(path).Replace("\\", "/") == searchFolder)
			.Select(path => AssetDatabase.LoadAssetAtPath<Object>(path))
			.ToArray();

		EditorUtility.SetDirty(this);
	}
#endif

	public TAsset GetAsset<TAsset>(string name) where TAsset : UnityEngine.Object {
		return (TAsset)resources.First(r => r.name == name);
	}

	private void OnEnable() {
		ResourceGroups.RegisterGroupByAddress(this);
		logger.LogInfo(LogModules.resource, "Registered resource group with address '{}'", groupAddress);
	}
}

