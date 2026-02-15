using NUnit.Framework;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Resources = SoulboundBackend.Core.Resource;

#nullable enable

//public static partial class ResourceGroups {
//    public sealed class DummyGroup : IResourceGroupDefinition<GameObject> {
//        public static readonly DummyGroup instance = new();
//        static DummyGroup() => SoulboundBackend.Core.Resource.ResourceManager.RegisterGroupDefinition<DummyGroup, GameObject>(instance);
//        public string address => "dummy/";
//		public string scriptableObjectName => "dummyGroup";
//    }
//}

//public class MockGroupDefinition : IResourceGroupDefinition<Texture2D> {
//    public string address => "mock_group";
//    public string scriptableObjectName => "MockGroup";
//}

//public class MockResourceGroup : ResourceGroup {
//    private Dictionary<string, UnityEngine.Object> fakeAssets = new();

//    public void AddFakeAsset<T>(string name, T asset) where T : UnityEngine.Object {
//        fakeAssets[name] = asset;
//    }

//    public new TAsset? GetAsset<TAsset>(string name) where TAsset : UnityEngine.Object {
//        if (fakeAssets.TryGetValue(name, out var asset)) {
//            return (TAsset)asset;
//        }
//        return null;
//    }
//}

//public class ResourceTests {
//	const string registryFolder = "Assets/Resources/Registry";
//	const string dummyGroupFolder = "DummyGroupFolder";
//	const string dummyGroupPath = "Assets/Resources/dummyGroup.asset";
//	private static string dummyGroupSearch = $"{registryFolder}/{dummyGroupFolder}";
//	private static string dummyPrefabPath = $"{dummyGroupSearch}/dummyObject.prefab";

//    [SetUp]
//    public void Setup() {
//        ResourceManager.UnloadAll();
//        ResourceManager.ClearDefinitions();

//        typeof(ResourceManager)
//            .GetField("registeredGroupDefinitions", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.SetValue(null, new Dictionary<Type, object>());

//        typeof(ResourceManager)
//            .GetField("addressesByGroupType", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.SetValue(null, new Dictionary<Type, string>());

//        typeof(ResourceManager)
//            .GetField("groupsByAddress", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.SetValue(null, new Dictionary<string, ResourceGroup>());
//    }

//    [TearDown]
//    public void CleanupDummy() {
//        AssetDatabase.DeleteAsset(dummyGroupPath);
//        AssetDatabase.DeleteAsset(dummyGroupSearch);
//        AssetDatabase.DeleteAsset("Assets/Resources/MockGroup.asset");
//    }

//    private ResourceGroup RegisterDummyGroup() {
//        Assert.That(!string.IsNullOrEmpty(AssetDatabase.CreateFolder(registryFolder, dummyGroupFolder)), () => $"Could not create folder {dummyGroupSearch}");
//        Assert.That(ResourceGroups.DummyGroup.instance != null);
//        try {
//            Resources.ResourceManager.GetAddressFromGroupDefinition<DummyGroup>();
//        } catch (KeyNotFoundException) {
//            Resources.ResourceManager.RegisterGroupDefinition<DummyGroup, GameObject>(DummyGroup.instance);
//        }
//        ResourceGroup group = ScriptableObject.CreateInstance<ResourceGroup>();
//        group.groupAddress = "dummy/";
//        group.searchFolder = dummyGroupSearch;
//        group.assetType = "GameObject";
//        AssetDatabase.CreateAsset(group, dummyGroupPath);
//        Assert.That(AssetDatabase.AssetPathExists(dummyGroupPath), () => "Could not create ResourceGroup asset");
//        try {
//            Resources.ResourceManager.RegisterGroupByAddress(group);
//        } catch (ArgumentException) { }
//        return group;
//    }

//    private GameObject SaveDummyObject(string name, string path) {
//        GameObject prefab = new GameObject(name);
//        PrefabUtility.SaveAsPrefabAsset(prefab, path);
//        AssetDatabase.Refresh();
//        Assert.That(AssetDatabase.AssetPathExists(path), () => "Could not create prefab asset");
//        return prefab;
//    }

//    [Test]
//	public void ResourceManager_ReturnsAsset_WhenAssetExistsInRegisteredGroup() {
//        ResourceGroup group = RegisterDummyGroup();

//        SaveDummyObject("dummyObject", dummyPrefabPath);
//		group.RefreshAddresses();

//		GameObject? loadedPrefab = ResourceManager.Get<GameObject, ResourceGroups.DummyGroup>("dummyObject");
//		Assert.That(loadedPrefab != null, () => "Failed to load prefab");

//		Resources.ResourceManager.UnloadGroup(group, typeof(ResourceGroups.DummyGroup));
//    }

//	[Test]
//	public void ResourceManager_ReturnsNull_WhenAssetDoesNotExist() {
//        var def = new MockGroupDefinition();
//        ResourceManager.RegisterGroupDefinition<MockGroupDefinition, Texture2D>(def);
//		LogAssert.ignoreFailingMessages = true;

//        var mockGroup = ScriptableObject.CreateInstance<MockResourceGroup>();
//        AssetDatabase.CreateAsset(mockGroup, "Assets/Resources/MockGroup.asset");
//        mockGroup.groupAddress = "mock/";
//        ResourceManager.RegisterGroupByAddress(mockGroup);

//        var nonexistent = ResourceManager.Get<Texture2D, MockGroupDefinition>("nonexistent_asset");

//		Assert.That(nonexistent == null, "Expected null for non-existent asset");
//    }

//    [Test]
//    public void RegisterGroupDefinition_ShouldStoreAddressAndDefinition() {
//        var def = new MockGroupDefinition();
//        ResourceManager.RegisterGroupDefinition<MockGroupDefinition, Texture2D>(def);

//        var addresses = (Dictionary<Type, string>)typeof(ResourceManager)
//            .GetField("addressesByGroupType", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.GetValue(null)!;
//        var defs = (Dictionary<Type, object>)typeof(ResourceManager)
//            .GetField("registeredGroupDefinitions", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.GetValue(null)!;

//        Assert.IsTrue(addresses.ContainsKey(typeof(MockGroupDefinition)));
//        Assert.IsTrue(defs.ContainsKey(typeof(MockGroupDefinition)));
//        Assert.AreEqual("mock_group", addresses[typeof(MockGroupDefinition)]);
//    }

//    [Test]
//    public void RegisterGroupDefinition_ShouldNotDuplicateOnReRegister() {
//        var def = new MockGroupDefinition();
//        ResourceManager.RegisterGroupDefinition<MockGroupDefinition, Texture2D>(def);
//        ResourceManager.RegisterGroupDefinition<MockGroupDefinition, Texture2D>(def); // should not overwrite

//        var addresses = (Dictionary<Type, string>)typeof(ResourceManager)
//            .GetField("addressesByGroupType", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.GetValue(null)!;

        
//        Assert.AreEqual(1, addresses.Count);
//    }

//    [Test]
//    public void RegisterGroupByAddress_ShouldAddGroupToCache() {
//        var mockGroup = ScriptableObject.CreateInstance<MockResourceGroup>();
//        mockGroup.groupAddress = "mock_group";

//        ResourceManager.RegisterGroupByAddress(mockGroup);

//        var groups = (Dictionary<string, ResourceGroup>)typeof(ResourceManager)
//            .GetField("groupsByAddress", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.GetValue(null)!;

//        Assert.AreEqual(1, groups.Count);
//        Assert.AreSame(mockGroup, groups["mock_group"]);
//    }

//    [Test]
//    public void GetGroupByAddress_ShouldReturnCorrectGroup() {
//        var mockGroup = ScriptableObject.CreateInstance<MockResourceGroup>();
//        mockGroup.groupAddress = "mock_group";

//        ResourceManager.RegisterGroupByAddress(mockGroup);
//        var result = ResourceManager.GetGroupByAddress("mock_group");

//        Assert.AreSame(mockGroup, result);
//    }

//    [Test]
//    public void UnloadGroup_ShouldRemoveGroupAndDefinition() {
//        var def = new MockGroupDefinition();
//        ResourceManager.RegisterGroupDefinition<MockGroupDefinition, Texture2D>(def);

//        var mockGroup = ScriptableObject.CreateInstance<MockResourceGroup>();
//        mockGroup.groupAddress = "mock_group";
//        ResourceManager.RegisterGroupByAddress(mockGroup);

//        ResourceManager.UnloadGroup(mockGroup, typeof(MockGroupDefinition));

//        var groups = (Dictionary<string, ResourceGroup>)typeof(ResourceManager)
//            .GetField("groupsByAddress", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.GetValue(null)!;
//        var addresses = (Dictionary<Type, string>)typeof(ResourceManager)
//            .GetField("addressesByGroupType", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.GetValue(null)!;

//        Assert.IsEmpty(groups);
//        Assert.IsEmpty(addresses);
//    }

//    [Test]
//    public void UnloadAll_ShouldClearAllGroups() {
//        var mockGroup = ScriptableObject.CreateInstance<MockResourceGroup>();
//        mockGroup.groupAddress = "mock_group";
//        ResourceManager.RegisterGroupByAddress(mockGroup);

//        ResourceManager.UnloadAll();

//        var groups = (Dictionary<string, ResourceGroup>)typeof(ResourceManager)
//            .GetField("groupsByAddress", BindingFlags.NonPublic | BindingFlags.Static)
//            ?.GetValue(null)!;

//        Assert.IsEmpty(groups);
//        Assert.IsFalse(ResourceManager.groupsPreloaded);
//    }

//    [Test]
//    public void Get_ShouldThrowIfGroupNotRegistered() {
//        var def = new MockGroupDefinition();
//        ResourceManager.RegisterGroupDefinition<MockGroupDefinition, Texture2D>(def);

//        Assert.Throws<NullReferenceException>(() => {
//            ResourceManager.Get<Texture2D, MockGroupDefinition>("missing");
//        });
//    }
//}
