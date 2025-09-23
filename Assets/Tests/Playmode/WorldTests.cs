using NUnit.Framework;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using SoulboundBackend.Tests;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class WorldTests {
	[Test]
	public void World_LoadsSuccessfully_WhenSceneProvided() {
		TestingEnvironment.Prepare(); 

		Scene scene = SceneManager.CreateScene(Guid.NewGuid().ToString());

		WorldManager worldManager = new("testSaves");
		worldManager.LoadWorld("test1", scene);

		Assert.That(LevelManager.instance.Level.isBootstrapped);
	}

	[UnityTest] 
	public IEnumerator World_LoadsSuccessfully_WhenNoSceneProvided() {
		TestingEnvironment.Prepare();

        WorldManager worldManager = new("testSaves");
        worldManager.LoadWorld("test1", null);
		yield return new WaitUntil(() => LevelManager.instance?.Level?.isBootstrapped ?? false);
		Assert.Pass();
    }

	[SetUp]
	public void PrepareEnvironment() {
        StaticResetManager.ResetAll();
        ResourceGroups.Bootstrap();
    }
}
