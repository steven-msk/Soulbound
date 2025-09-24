using NUnit.Framework;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using SoulboundBackend.Tests;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

#nullable enable

public class WorldTests {
	internal class WorldContextResult {
		public WorldManager? worldManager { get; set; }
	}

	internal static IEnumerator CreateScenelessContext(
			WorldContextResult result,
			string savesRoot = "testWorlds",
			string world = "test", 
			ISaveStrategy<WorldDump>? saveStrategy = null
		) {
		result.worldManager = new(savesRoot, saveStrategy ?? new DoNotSaveWorldStrategy());
        result.worldManager.LoadWorld(world, null);
        yield return new WaitUntil(
			() => result.worldManager.activeLevelManager?.Level.isBootstrapped ?? false
		);
    }

	internal static void CreateScenedContext(
		Scene scene,
		out WorldManager worldManager,
		string savesRoot = "testWorlds", 
		string world = "test", 
		ISaveStrategy<WorldDump>? saveStrategy = null
		) {
		worldManager = new(savesRoot, saveStrategy ?? new DoNotSaveWorldStrategy());
		worldManager.LoadWorld(world, scene);
	}

	[Test]
	public void World_LoadsSuccessfully_WhenSceneProvided() {
		Scene scene = SceneManager.CreateScene(Guid.NewGuid().ToString());
		CreateScenedContext(scene, out var worldManager);

		Assert.That(
			worldManager.activeLevelManager?.Level.isBootstrapped ?? false,
			() => "Failed to create scened world"
		);
	}

	[UnityTest] 
	public IEnumerator World_LoadsSuccessfully_WhenNoSceneProvided() {
		var result = new WorldContextResult();
		yield return WorldTests.CreateScenelessContext(result);
		Assert.That(result.worldManager != null, () => "Failed to create sceneless world");
    }

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
