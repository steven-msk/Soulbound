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
using Zenject;

#nullable enable

namespace WorldTests {
	[TestFixture]
	public class World {
		internal const string commonSavesRoot = "testSaves";

		internal static string CreateNewWorldID() {
			return "world_" + Guid.NewGuid().ToString();
		}

		private static WorldManager CreateManagerInstance(ISaveStrategy<WorldDump>? saveStrategy) {
			return new WorldManager(
				commonSavesRoot,
				saveStrategy ?? new DoNotSaveWorldStrategy(),
				() => Application.temporaryCachePath
			);
		}

		internal static IEnumerator CreateContextWithNoSceneProvided(
				ContextBox<WorldManager> worldBox,
				string? world = null,
				ISaveStrategy<WorldDump>? saveStrategy = null
			) {
			worldBox.value = new WorldManager(commonSavesRoot,
				saveStrategy ?? new DoNotSaveWorldStrategy(),
				() => Application.temporaryCachePath
			);
			worldBox.value.LoadWorld(
				world ?? CreateNewWorldID(),
				initPlayerState: false,
				() => {
					var prefab = ResourceManager.GetRuntimePrefab("sceneContext");
					var sceneContext = GameObject.Instantiate(prefab)!.GetComponent<SceneContext>();

					sceneContext.AddNormalInstaller(new LevelInstallerWrapper(() => worldBox.value));
					sceneContext.Run();
					return sceneContext;
				},
				PlayModeTesting.CreateNewTestScene
			);
			yield return new WaitUntil(
				() => worldBox.value.activeLevelManager?.Level.isWorldLoaded ?? false
			);
		}

		internal static IEnumerator CreateAnonymousContext(ContextBox<WorldManager> worldBox) {
			yield return World.CreateContextWithNoSceneProvided(worldBox, World.CreateNewWorldID());
		}

		internal static IEnumerator CreateAnonymousContext(
				Scene? worldScene,
				ContextBox<WorldManager> worldBox
			) {
			yield return CreateContextWithSceneProvided(
				worldScene ?? PlayModeTesting.CreateNewTestScene(), worldBox
			);
		}

		internal static IEnumerator CreateContextWithSceneProvided(
				Scene scene,
				ContextBox<WorldManager> worldBox,
				string? world = null,
				ISaveStrategy<WorldDump>? saveStrategy = null
			) {
			worldBox.value = CreateManagerInstance(saveStrategy);

			worldBox.value.LoadWorld(
				world ?? CreateNewWorldID(),
				initPlayerState: false,
				() => {
					var sceneContext = scene.GetRootGameObjects()
						.SelectMany(o => o.GetComponentsInChildren<SceneContext>(true))
						.FirstOrDefault();

					if (sceneContext == null) {
						var prefab = ResourceManager.GetRuntimePrefab("sceneContext");
						sceneContext = GameObject.Instantiate(prefab)!.GetComponent<SceneContext>();
						sceneContext.AddNormalInstaller(new LevelInstallerWrapper(() => worldBox.value));
					}

					sceneContext.Run();
					return sceneContext;
				},
				() => scene
			);
			yield return new WaitUntil(
				() => worldBox.value.activeLevelManager?.Level.isWorldLoaded ?? false
			);
		}

		internal static IEnumerator CreateSavedContext(
				ContextBox<WorldManager> worldBox,
				ContextBox<Scene> sceneBox, 
				string world
			) {
			sceneBox.value = PlayModeTesting.CreateNewTestScene();
			yield return CreateContextWithSceneProvided(
				sceneBox.value, worldBox, world, 
				new WorldSaveStrategy()
			);
		}

		internal static Level TryGetLevel(WorldManager? worldManager) {
			return worldManager?.activeLevelManager?.Level
				?? throw new ArgumentException("Level reference wasnt loaded");
		}

		internal static Level TryGetLevel(ContextBox<WorldManager> worldBox) {
			return TryGetLevel(worldBox.value);
		}

		[UnityTest]
		public IEnumerator World_LoadsSuccessfully_WhenSceneProvided() {
			Scene scene = PlayModeTesting.CreateNewTestScene();
			var worldBox = new ContextBox<WorldManager>();
			yield return CreateContextWithSceneProvided(scene, worldBox);

			Assert.That(
				worldBox.value?.activeLevelManager?.Level.isWorldLoaded ?? false,
				() => "Failed to create world with provided scene"
			);
		}

		[UnityTest]
		public IEnumerator World_LoadsSuccessfully_WhenNoSceneProvided() {
			var worldBox = new ContextBox<WorldManager>();
			yield return World.CreateContextWithNoSceneProvided(worldBox);
			Assert.That(worldBox.value != null, () => "Failed to create sceneless world");
		}

		[UnityTest]
		public IEnumerator World_SaveAndReload_PersistsBlockChanges() {
			string world = CreateNewWorldID();

			var sceneBox = new ContextBox<Scene>();
			var worldBox = new ContextBox<WorldManager>();
			yield return CreateSavedContext(worldBox, sceneBox, world);

			Level level = TryGetLevel(worldBox);
			WorldDump dump = level.CreateDump();
			ChunkBlockPos pos = new(0, 0, 0);

			pos.chunkX = dump.generatedChunks[0].xpos;
			Block blockAtPos = dump.generatedChunks[0].BlockStateAt(pos).block;
			Block target = Blocks.wood;

			dump.generatedChunks[0].SetBlock(pos, target.defaultState);
			worldBox.value!.SaveWorld(world, dump);

			yield return PlayModeTesting.UnloadSceneAsync(sceneBox.value);

			StaticResetManager.ResetAll();
			yield return CreateSavedContext(worldBox, sceneBox, world);

			level = TryGetLevel(worldBox);
			Assert.That(level.BlockAt(pos.ToWorldBlockPos()) == target);
		}

		[UnityTest]
		public IEnumerator World_HasUniqueSaveFile() {
			string world1 = CreateNewWorldID();
			string world2 = CreateNewWorldID();

			var sceneBox = new ContextBox<Scene>();
			var worldBox = new ContextBox<WorldManager>();
			yield return CreateSavedContext(worldBox, sceneBox, world1);
			int world1seed = TryGetLevel(worldBox).seed;

			yield return worldBox.value!.TerminateWorldProcess(sceneBox.value, world1,
				() => TryGetLevel(worldBox).CreateDump());

			yield return CreateSavedContext(worldBox, sceneBox, world2);
			int world2seed = TryGetLevel(worldBox).seed;

			yield return worldBox.value!.TerminateWorldProcess(sceneBox.value, world2,
				() => TryGetLevel(worldBox).CreateDump());

			yield return CreateSavedContext(worldBox, sceneBox, world1);
			Assert.That(world1seed, Is.EqualTo(TryGetLevel(worldBox).seed));

			yield return PlayModeTesting.UnloadSceneAsync(sceneBox.value);

			yield return CreateSavedContext(worldBox, sceneBox, world2);
			Assert.That(world2seed, Is.EqualTo(TryGetLevel(worldBox).seed));
		}

		[UnityTest]
		public IEnumerator World_LevelInstanceChanges_WhenSwitchingWorlds() {
			string world1 = CreateNewWorldID();
			string world2 = CreateNewWorldID();
			var worldBox = new ContextBox<WorldManager>();
			var sceneBox = new ContextBox<Scene>();

			yield return CreateSavedContext(worldBox, sceneBox, world1);
			Level world1instance = TryGetLevel(worldBox);
			worldBox.value!.SaveWorld(world1, world1instance.CreateDump());

			yield return PlayModeTesting.UnloadSceneAsync(sceneBox.value);

			yield return CreateSavedContext(worldBox, sceneBox, world2);
			Level world2instance = TryGetLevel(worldBox);

			Assert.That(world1instance, Is.Not.EqualTo(world2instance));
		}

		[UnityTest]
		public IEnumerator DoNotSaveWorldStrategy_DiscardsSaves() {
			string world = CreateNewWorldID();
			var worldBox = new ContextBox<WorldManager>();

			Scene scene = PlayModeTesting.CreateNewTestScene();
			yield return CreateAnonymousContext(scene, worldBox);
			int seed = TryGetLevel(worldBox).seed;
			worldBox.value!.SaveWorld(world, TryGetLevel(worldBox).CreateDump());

			yield return PlayModeTesting.UnloadSceneAsync(scene);

			yield return CreateContextWithNoSceneProvided(worldBox, world, new DoNotSaveWorldStrategy());
			Assert.That(seed, Is.Not.EqualTo(TryGetLevel(worldBox).seed));
		}
	}
}

namespace WorldTests.BlockTests {
	[TestFixture]
	public class Block {
		[UnityTest]
		public IEnumerator BlockState_GetsPlacedSuccessfully_WhenWorldJustBootstrapped() {
			var result = new ContextBox<WorldManager>();
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
			var sceneBox = new ContextBox<Scene>();
			var worldBox = new ContextBox<WorldManager>();

			yield return World.CreateSavedContext(worldBox, sceneBox, world);

			BlockState? targetState = World.TryGetLevel(worldBox).BlockStateAt(targetPos);
			worldBox.value!.SaveWorld(world, World.TryGetLevel(worldBox).CreateDump());
			yield return PlayModeTesting.UnloadSceneAsync(sceneBox.value);

			yield return World.CreateSavedContext(worldBox, sceneBox, world);
			BlockState? actualState = World.TryGetLevel(worldBox).BlockStateAt(targetPos);
			Assert.That(actualState, Is.EqualTo(targetState));
		}

		[UnityTest]
		public IEnumerator BlockState_ReplacesCorrectly_WhenSetAgain() {
			var result = new ContextBox<WorldManager>();
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
			var result = new ContextBox<WorldManager>();
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
			var result = new ContextBox<WorldManager>();
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