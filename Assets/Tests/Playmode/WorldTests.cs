using NUnit.Framework;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
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
		Scene scene = SceneManager.CreateScene(Guid.NewGuid().ToString());

		WorldManager worldManager = new("testSaves", new DoNotSaveWorldStrategy());
		worldManager.LoadWorld("test1", scene);

		Assert.That(LevelManager.instance.Level.isBootstrapped);
	}

	[UnityTest] 
	public IEnumerator World_LoadsSuccessfully_WhenNoSceneProvided() {
        WorldManager worldManager = new("testSaves", new DoNotSaveWorldStrategy());
        worldManager.LoadWorld("test1", null);
		yield return new WaitUntil(() => LevelManager.instance?.Level?.isBootstrapped ?? false);
		Assert.Pass();
	[UnityTest]
	public IEnumerator BlockState_GetsPlacedSuccessfully_WhenWorldJustBootstrapped() {
		var result = new WorldContextResult();
		yield return WorldTests.CreateScenelessContext(result);
			
		Level level = result.worldManager?.activeLevelManager?.Level
			?? throw new ArgumentException("Scened world didnt load properly");
		BlockPos blockPos = new(0, 0);

		level.SetBlock(blockPos, Blocks.air.defaultState);
		Assert.That(level.BlockAt(blockPos) == Blocks.air);
    }

	[SetUp]
	public void PrepareEnvironment() {
        StaticResetManager.ResetAll();
        ResourceGroups.Bootstrap();
    }
}
