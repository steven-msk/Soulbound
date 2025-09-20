using System.Collections;
using NUnit.Framework;
using SoulboundBackend.Core.Resource;
using UnityEngine;
using UnityEngine.TestTools;

#nullable enable

public class ResourceTests {
    [Test]
    public void ResourceManagerLoadsPrefab() {
        GameObject? prefab = ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("ai test");
        Assert.That(prefab != null, () => "Failed to load prefab");
	}
}
