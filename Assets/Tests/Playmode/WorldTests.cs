using JetBrains.Annotations;
using NUnit.Framework;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using SoulboundBackend.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

#nullable enable

public class WorldTests {
	internal class WorldContextResult {
		public WorldManager? worldManager { get; set; }
	}
	internal const string commonSavesRoot = "testSaves";

	internal static string CreateNewWorldID() {
		return "world " + Guid.NewGuid().ToString();
    }

	internal static IEnumerator CreateContextWithNoSceneProvided(
			WorldContextResult result,
			string? world = null, 
			ISaveStrategy<WorldDump>? saveStrategy = null
		) {
		result.worldManager = new(commonSavesRoot, 
			saveStrategy ?? new DoNotSaveWorldStrategy(),
			() => Application.temporaryCachePath
		);
		result.worldManager.LoadWorld(world ?? CreateNewWorldID(), null);
		yield return new WaitUntil(
			() => result.worldManager.activeLevelManager?.Level.isBootstrapped ?? false
		);
		yield return null;
	}

	internal static void CreateContextWithSceneProvided(
		Scene scene,
		out WorldManager worldManager,
		string? world = null, 
		ISaveStrategy<WorldDump>? saveStrategy = null
		) {
		worldManager = new(commonSavesRoot, 
			saveStrategy ?? new DoNotSaveWorldStrategy(),
			() => Application.temporaryCachePath
		);
		worldManager.LoadWorld(world ?? CreateNewWorldID(), scene);
	}

	internal static Scene CreateSavedContext(out WorldManager worldManager, string world) {
        Scene scene = TestingEnvironment.CreateNewTestScene();
        CreateContextWithSceneProvided(scene, out worldManager, world, new WorldSaveStrategy());
        return scene;
    }

	internal static Level TryGetLevel(WorldManager? worldManager) {
		return worldManager?.activeLevelManager?.Level
			?? throw new ArgumentException("Scened world didnt load properly");
    }

    [Test]
	public void World_LoadsSuccessfully_WhenSceneProvided() {
		Scene scene = TestingEnvironment.CreateNewTestScene();
		CreateContextWithSceneProvided(scene, out var worldManager);

		Assert.That(
			worldManager.activeLevelManager?.Level.isBootstrapped ?? false,
			() => "Failed to create scened world"
		);
	}

	[UnityTest] 
	public IEnumerator World_LoadsSuccessfully_WhenNoSceneProvided() {
		var result = new WorldContextResult();
		yield return WorldTests.CreateContextWithNoSceneProvided(result);
		Assert.That(result.worldManager != null, () => "Failed to create sceneless world");
	}

	[UnityTest]
	public IEnumerator World_SaveAndReload_PersistsBlockChanges() {
		string world = CreateNewWorldID();

		Scene scene = CreateSavedContext(out var worldManager, world);
		yield return null;

		Level level = TryGetLevel(worldManager);
		WorldDump dump = level.Save();
		ChunkBlockPos pos = new(0, 0, 0);

		pos.chunkX = dump.generatedChunks[0].xpos;
		Block blockAtPos = dump.generatedChunks[0].BlockStateAt(pos).block;
		Block target = Blocks.AllBlocks().First(block => blockAtPos != block);

		dump.generatedChunks[0].SetBlock(pos, target.defaultState);
		worldManager.SaveWorld(world, dump);

		AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
		yield return new WaitUntil(() => async.isDone);
		yield return null;

		StaticResetManager.ResetAll();
		CreateSavedContext(out worldManager, world);

		level = TryGetLevel(worldManager);
		Assert.That(level.BlockAt(pos.ToWorldBlockPos()) == target);
	}

	[UnityTest]
	public IEnumerator World_HasUniqueSaveFile() {
		string world1 = CreateNewWorldID();
		string world2 = CreateNewWorldID();

		Scene scene = CreateSavedContext(out var worldManager, world1);
		int world1seed = TryGetLevel(worldManager).seed;
		worldManager.SaveWorld(world1, TryGetLevel(worldManager).Save());

		yield return TestingEnvironment.UnloadSceneAsync(scene);

		scene = CreateSavedContext(out worldManager, world2);
		int world2seed = TryGetLevel(worldManager).seed;
        worldManager.SaveWorld(world2, TryGetLevel(worldManager).Save());

        yield return TestingEnvironment.UnloadSceneAsync(scene);


        scene = CreateSavedContext(out worldManager, world1);
		Assert.That(world1seed, Is.EqualTo(TryGetLevel(worldManager).seed));

        yield return TestingEnvironment.UnloadSceneAsync(scene);

		scene = CreateSavedContext(out worldManager, world2);
		Assert.That(world2seed, Is.EqualTo(TryGetLevel(worldManager).seed));

		yield return TestingEnvironment.UnloadSceneAsync(scene);
    }

	[UnityTest]
	public IEnumerator World_LevelInstanceChanges_WhenSwitchingWorlds() {
		string world1 = CreateNewWorldID();
		string world2 = CreateNewWorldID();

		Scene scene = CreateSavedContext(out var worldManager, world1);
		Level world1instance = TryGetLevel(worldManager);
		worldManager.SaveWorld(world1, world1instance.Save());

		yield return TestingEnvironment.UnloadSceneAsync(scene);

		scene = CreateSavedContext(out worldManager, world2);
		Level world2instance = TryGetLevel(worldManager);

		Assert.That(world1instance, Is.Not.EqualTo(world2instance));
	}

	[UnityTest]
	public IEnumerator DoNotSaveWorldStrategy_DiscardsSaves() {
		string world = CreateNewWorldID();

		Scene scene = TestingEnvironment.CreateNewTestScene();
		CreateContextWithSceneProvided(scene, out var worldManager, world, saveStrategy: new DoNotSaveWorldStrategy());
		int seed = TryGetLevel(worldManager).seed;
		worldManager.SaveWorld(world, TryGetLevel(worldManager).Save());

		yield return TestingEnvironment.UnloadSceneAsync(scene);

		var result = new WorldContextResult();
		yield return CreateContextWithNoSceneProvided(result, world, saveStrategy: new DoNotSaveWorldStrategy());
		Assert.That(seed, Is.Not.EqualTo(TryGetLevel(result.worldManager).seed));
	}

	[UnityTest]
	public IEnumerator BlockState_GetsPlacedSuccessfully_WhenWorldJustBootstrapped() {
		var result = new WorldContextResult();
		yield return WorldTests.CreateContextWithNoSceneProvided(result);
			
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
