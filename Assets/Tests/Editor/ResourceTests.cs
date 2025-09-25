using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using SoulboundBackend.Core.Resource;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

#nullable enable
public class ResourceTests {
	const string registryFolder = "Assets/Resources/Registry";
	const string dummyGroupFolder = "DummyGroupFolder";
	const string dummyGroupPath = "Assets/Resources/dummyGroup.asset";
	private static string dummyGroupSearch = $"{registryFolder}/{dummyGroupFolder}";
	private static string dummyPrefabPath = $"{dummyGroupSearch}/dummyObject.prefab";

	[Test]
	public void ResourceManager_ReturnsAsset_WhenAssetExistsInRegisteredGroup() {
		Assert.That(!string.IsNullOrEmpty(AssetDatabase.CreateFolder(registryFolder, dummyGroupFolder)), () => $"Could not create folder {dummyGroupSearch}");

		ResourceGroup group = ScriptableObject.CreateInstance<ResourceGroup>();
		group.groupAddress = "dummy/";
		group.searchFolder = dummyGroupSearch;
		group.assetType = "GameObject";
		AssetDatabase.CreateAsset(group, dummyGroupPath);
		Assert.That(AssetDatabase.AssetPathExists(dummyGroupPath), () => "Could not create ResourceGroup asset");
		try {
			SoulboundBackend.Core.Resource.ResourceGroups.RegisterGroupByAddress(group);
		} catch (ArgumentException) { }

		GameObject prefab = new GameObject("dummyObject");
		PrefabUtility.SaveAsPrefabAsset(prefab, dummyPrefabPath);
		AssetDatabase.Refresh();
		Assert.That(AssetDatabase.AssetPathExists(dummyPrefabPath), () => "Could not create prefab asset");
		group.RefreshGroup();

		GameObject? loadedPrefab = ResourceManager.Get<GameObject, ResourceGroups.DummyGroup>("dummyObject");
		Assert.That(loadedPrefab != null, () => "Failed to load prefab");
	}

	[Test]
	public void ResourceManager_ReturnsNull_WhenAssetDoesNotExist() {
		SoulboundBackend.Core.Resource.ResourceGroups.Bootstrap();
		LogAssert.ignoreFailingMessages = true;
        GameObject? prefab = ResourceManager.Get<GameObject, SoulboundBackend.Core.Resource.ResourceGroups.Runtime.Prefabs>("unknownPrefab");
		Assert.That(prefab == null, "Expected null for non-existent asset");
	}

	[OneTimeTearDown]
	public void Cleanup() {
		AssetDatabase.DeleteAsset(dummyGroupPath);
		AssetDatabase.DeleteAsset(dummyGroupSearch);
	}
}

#if UNITY_INCLUDE_TESTS
public static partial class ResourceGroups {
	public sealed class DummyGroup : IResourceGroupDefinition<GameObject> {
		public static readonly DummyGroup instance = new();
		static DummyGroup() => SoulboundBackend.Core.Resource.ResourceGroups.RegisterGroupDefinition<DummyGroup, GameObject>(instance);
		public string address => "dummy/";
	}
}
#endif
