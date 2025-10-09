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

namespace WorldTests {
	[TestFixture]
	public class World {
		internal const string commonSavesRoot = "testSaves";

		internal static string CreateNewWorldID() {
			return "world_" + Guid.NewGuid().ToString();
		}

		internal static IEnumerator CreateContextWithNoSceneProvided(
				ContextResult<WorldManager> result,
				string? world = null,
				ISaveStrategy<WorldDump>? saveStrategy = null,
                Func<BootstrapTreeBuilder, IEnumerable<IBootstrappable>>? bootstrapTree = null
            ) {
			result.value = new WorldManager(commonSavesRoot,
				saveStrategy ?? new DoNotSaveWorldStrategy(),
				() => Application.temporaryCachePath
			);
			result.value.LoadWorld(
				world ?? CreateNewWorldID(),
				levelScene: null,
				bootstrapTree ?? (treeBuilder => new List<IBootstrappable>()),
				initPlayerState: false
			);
			yield return new WaitUntil(
				() => result.value.activeLevelManager?.Level.isBootstrapped ?? false
			);
			yield return null;
		}

		internal static IEnumerator CreateAnonymousContext() {
            var result = new ContextResult<WorldManager>();
            yield return World.CreateContextWithNoSceneProvided(result, World.CreateNewWorldID());
        }

		internal static void CreateAnonymousContext(
				Scene? worldScene, 
				out WorldManager worldManager
			) {
			World.CreateContextWithSceneProvided(
				worldScene ?? PlayModeTesting.CreateNewTestScene(),
				out worldManager
			);
		}

		internal static void CreateContextWithSceneProvided(
				Scene scene,
				out WorldManager worldManager,
				string? world = null,
				ISaveStrategy<WorldDump>? saveStrategy = null,
                Func<BootstrapTreeBuilder, IEnumerable<IBootstrappable>>? bootstrapTree = null
            ) {
			worldManager = new(commonSavesRoot,
				saveStrategy ?? new DoNotSaveWorldStrategy(),
				() => Application.temporaryCachePath
			);
			worldManager.LoadWorld(
				world ?? CreateNewWorldID(), 
				scene, 
				bootstrapTree ?? (treeBuilder => new List<IBootstrappable>()),
				initPlayerState: false
			);
		}

		internal static Scene CreateSavedContext(out WorldManager worldManager, string world) {
			Scene scene = PlayModeTesting.CreateNewTestScene();
			CreateContextWithSceneProvided(scene, out worldManager, world, new WorldSaveStrategy());
			return scene;
		}

		internal static Level TryGetLevel(WorldManager? worldManager) {
			return worldManager?.activeLevelManager?.Level
				?? throw new ArgumentException("Scened world didnt load properly");
		}

		[Test]
		public void World_LoadsSuccessfully_WhenSceneProvided() {
			Scene scene = PlayModeTesting.CreateNewTestScene();
			CreateContextWithSceneProvided(scene, out var worldManager);

			Assert.That(
				worldManager.activeLevelManager?.Level.isBootstrapped ?? false,
				() => "Failed to create scened world"
			);
		}

		[UnityTest]
		public IEnumerator World_LoadsSuccessfully_WhenNoSceneProvided() {
			var result = new ContextResult<WorldManager>();
			yield return World.CreateContextWithNoSceneProvided(result);
			Assert.That(result.value != null, () => "Failed to create sceneless world");
		}

		[UnityTest]
		public IEnumerator World_SaveAndReload_PersistsBlockChanges() {
			string world = CreateNewWorldID();

			Scene scene = CreateSavedContext(out var worldManager, world);
			yield return null;

			Level level = TryGetLevel(worldManager);
			WorldDump dump = level.CreateDump();
			ChunkBlockPos pos = new(0, 0, 0);

			pos.chunkX = dump.generatedChunks[0].xpos;
			Block blockAtPos = dump.generatedChunks[0].BlockStateAt(pos).block;
			Block target = Blocks.wood;

			dump.generatedChunks[0].SetBlock(pos, target.defaultState);
			worldManager.SaveWorld(world, dump);

			yield return PlayModeTesting.UnloadSceneAsync(scene);

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

			yield return worldManager.TerminateWorldProcess(scene, world1,
				() => TryGetLevel(worldManager).CreateDump());

			scene = CreateSavedContext(out worldManager, world2);
			int world2seed = TryGetLevel(worldManager).seed;

			yield return worldManager.TerminateWorldProcess(scene, world2,
				() => TryGetLevel(worldManager).CreateDump());

			scene = CreateSavedContext(out worldManager, world1);
			Assert.That(world1seed, Is.EqualTo(TryGetLevel(worldManager).seed));

			yield return PlayModeTesting.UnloadSceneAsync(scene);

			scene = CreateSavedContext(out worldManager, world2);
			Assert.That(world2seed, Is.EqualTo(TryGetLevel(worldManager).seed));
		}

		[UnityTest]
		public IEnumerator World_LevelInstanceChanges_WhenSwitchingWorlds() {
			string world1 = CreateNewWorldID();
			string world2 = CreateNewWorldID();

			Scene scene = CreateSavedContext(out var worldManager, world1);
			Level world1instance = TryGetLevel(worldManager);
			worldManager.SaveWorld(world1, world1instance.CreateDump());

			yield return PlayModeTesting.UnloadSceneAsync(scene);

			scene = CreateSavedContext(out worldManager, world2);
			Level world2instance = TryGetLevel(worldManager);

			Assert.That(world1instance, Is.Not.EqualTo(world2instance));
		}

		[UnityTest]
		public IEnumerator DoNotSaveWorldStrategy_DiscardsSaves() {
			string world = CreateNewWorldID();

			Scene scene = PlayModeTesting.CreateNewTestScene();
			CreateContextWithSceneProvided(scene, out var worldManager, world, new DoNotSaveWorldStrategy());
			int seed = TryGetLevel(worldManager).seed;
			worldManager.SaveWorld(world, TryGetLevel(worldManager).CreateDump());

			yield return PlayModeTesting.UnloadSceneAsync(scene);

			var result = new ContextResult<WorldManager>();
			yield return CreateContextWithNoSceneProvided(result, world, new DoNotSaveWorldStrategy());
			Assert.That(seed, Is.Not.EqualTo(TryGetLevel(result.value).seed));
		}

		[SetUp]
		public void PrepareEnvironment() {
			StaticResetManager.ResetAll();
		}
	}
}

namespace WorldTests.BlockTests {
	[TestFixture]
	public class Block {
		[UnityTest]
		public IEnumerator BlockState_GetsPlacedSuccessfully_WhenWorldJustBootstrapped() {
			var result = new ContextResult<WorldManager>();
			yield return World.CreateContextWithNoSceneProvided(result);

			Level level = result.value?.activeLevelManager?.Level
				?? throw new ArgumentException("Scened world didnt load properly");
			BlockPos blockPos = new(0, 0);

			level.SetBlock(blockPos, Blocks.air.defaultState);
			Assert.That(level.BlockAt(blockPos) == Blocks.air);
		}

		[UnityTest]
		public IEnumerator BlockState_PersistsCorrectly_AfterSaveAndReload() {
			string world = World.CreateNewWorldID();
			BlockPos targetPos = new(0, 0);

			Scene scene = World.CreateSavedContext(out var worldManager, world);

			BlockState? targetState = World.TryGetLevel(worldManager).BlockStateAt(targetPos);
			worldManager.SaveWorld(world, World.TryGetLevel(worldManager).CreateDump());
			yield return PlayModeTesting.UnloadSceneAsync(scene);

			World.CreateSavedContext(out worldManager, world);
			BlockState? actualState = World.TryGetLevel(worldManager).BlockStateAt(targetPos);
			Assert.That(actualState, Is.EqualTo(targetState));
		}

		[UnityTest]
		public IEnumerator BlockState_ReplacesCorrectly_WhenSetAgain() {
			var result = new ContextResult<WorldManager>();
			yield return World.CreateContextWithNoSceneProvided(result);

			Level level = World.TryGetLevel(result.value);
			BlockPos pos = new(2, 3);

			level.SetBlock(pos, Blocks.leaves.defaultState);
			Assert.That(level.BlockAt(pos) == Blocks.leaves);

			level.SetBlock(pos, Blocks.stone.defaultState);
			Assert.That(level.BlockAt(pos) == Blocks.stone);
		}

		[UnityTest]
		public IEnumerator BlockState_UpdatesChunkData_WhenPlaced() {
			var result = new ContextResult<WorldManager>();
			yield return World.CreateContextWithNoSceneProvided(result);

			Level level = World.TryGetLevel(result.value);
			ChunkBlockPos pos = new(5, 0, 0);
			pos.chunkX = level.CreateDump().generatedChunks[0].xpos;

			level.SetBlock(pos.ToWorldBlockPos(), Blocks.leaves.defaultState);

			var dump = level.CreateDump();
			var blockInDump = dump.generatedChunks[0].BlockStateAt(pos).block;

			Assert.That(blockInDump == Blocks.leaves);
		}

		[UnityTest]
		public IEnumerator BlockStates_MultipleBlocksRemainConsistent_InSameChunk() {
			var result = new ContextResult<WorldManager>();
			yield return World.CreateContextWithNoSceneProvided(result);

			Level level = World.TryGetLevel(result.value);
			BlockPos pos1 = new(0, 0);
			BlockPos pos2 = new(1, 0);

			level.SetBlock(pos1, Blocks.stone.defaultState);
			level.SetBlock(pos2, Blocks.dirt.defaultState);

			Assert.That(level.BlockAt(pos1) == Blocks.stone);
			Assert.That(level.BlockAt(pos2) == Blocks.dirt);
		}
	}
}