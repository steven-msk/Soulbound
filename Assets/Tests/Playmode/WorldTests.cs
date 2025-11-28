using JetBrains.Annotations;
using NUnit.Framework;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using SoulboundBackend.Core.Serialization;
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

		private static WorldManager CreateManagerInstance(IWorldSaveStrategy? saveStrategy) {
			ISerializer<WorldDump> worldSerializer = new JsonSerializer<WorldDump>(Soulbound.globalJsonSettings);
			SerializationPipeline<WorldDump> worldSerializationPipeline = new(worldSerializer);
			return new WorldManager(
				new WorldSerializationService(
					saveStrategy ?? new DoNotSaveWorldStrategy(),
					worldSerializationPipeline
				)
			);
		}

		internal static IEnumerator CreateContextWithNoSceneProvided(
				ContextBox<WorldManager> worldBox,
				string? world = null,
				IWorldSaveStrategy? saveStrategy = null
			) {
			worldBox.value = CreateManagerInstance(new DoNotSaveWorldStrategy());
			worldBox.value.LoadWorld(
				world ?? World.CreateNewWorldID(),
				() => {
					var prefab = ResourceManager.GetRuntimePrefab("sceneContext");
					var canvas = GameObject.Instantiate(ResourceManager.GetRuntimePrefab("Canvas")).GetComponent<Canvas>();
					var sceneContext = GameObject.Instantiate(prefab)!.GetComponent<SceneContext>();

					sceneContext.AddNormalInstaller(new LevelInstaller(worldBox.value, canvas));
					sceneContext.Run();
					return sceneContext;
				},
				() => PlayModeTesting.CreateNewSceneAndSetActive()
			);
			yield return new WaitUntil(
				() => worldBox.value.activeLevelManager?.level.isWorldLoaded ?? false
			);
		}

		internal static IEnumerator CreateAnonymousContext(ContextBox<WorldManager> worldBox) {
			yield return World.CreateContextWithNoSceneProvided(worldBox, World.CreateNewWorldID());
		}

		internal static IEnumerator CreateAnonymousContext(
				Scene? worldScene,
				ContextBox<WorldManager> worldBox
			) {
			yield return World.CreateContextWithSceneProvided(
				worldScene ?? PlayModeTesting.CreateNewTestScene(), worldBox
			);
		}

		internal static IEnumerator CreateContextWithSceneProvided(
				Scene scene,
				ContextBox<WorldManager> worldBox,
				string? world = null,
				IWorldSaveStrategy? saveStrategy = null
			) {
			worldBox.value = World.CreateManagerInstance(saveStrategy);

			worldBox.value.LoadWorld(
				world ?? World.CreateNewWorldID(),
				() => {
					var sceneContext = scene.GetRootGameObjects()
						.SelectMany(o => o.GetComponentsInChildren<SceneContext>(true))
						.FirstOrDefault();

					if (sceneContext == null) {
						var prefab = ResourceManager.GetRuntimePrefab("sceneContext");
						sceneContext = GameObject.Instantiate(prefab)!.GetComponent<SceneContext>();
						var canvas = GameObject.Instantiate(ResourceManager.GetRuntimePrefab("Canvas")).GetComponent<Canvas>();
						sceneContext.AddNormalInstaller(new LevelInstaller(worldBox.value, canvas));
					}

					sceneContext.Run();
					return sceneContext;
				},
				() => SceneManager.SetActiveScene(scene)
			);
			yield return new WaitUntil(
				() => worldBox.value.activeLevelManager?.level.isWorldLoaded ?? false
			);
		}

		internal static IEnumerator CreateSavedContext(
				ContextBox<WorldManager> worldBox,
				ContextBox<Scene> sceneBox, 
				string world
			) {
			sceneBox.value = PlayModeTesting.CreateNewTestScene();
			yield return World.CreateContextWithSceneProvided(
				sceneBox.value, worldBox, world, 
				new WorldSaveStrategy("saves", Application.temporaryCachePath)
			);
		}

		internal static Level TryGetLevel(WorldManager? worldManager) {
			return World.TryGetManager(worldManager)?.level!;
		}

		internal static LevelManager TryGetManager(WorldManager? worldManager) {
			return worldManager?.activeLevelManager
				?? throw new ArgumentException("Level reference wasnt loaded");
		}

		internal static Level TryGetLevel(ContextBox<WorldManager> worldBox) {
			return World.TryGetLevel(worldBox.value);
		}

		internal static LevelManager TryGetManager(ContextBox<WorldManager> worldBox) {
			return World.TryGetManager(worldBox.value);
		}

		[UnityTest]
		public IEnumerator TestRelevant_World_LoadsSuccessfully_WhenSceneProvided() {
			Scene scene = PlayModeTesting.CreateNewTestScene();
			var worldBox = new ContextBox<WorldManager>();
			yield return World.CreateContextWithSceneProvided(scene, worldBox);

			Assert.That(
				worldBox.value?.activeLevelManager?.level.isWorldLoaded ?? false,
				() => "Failed to create world with provided scene"
			);
		}

		[UnityTest]
		public IEnumerator TestRelevant_World_LoadsSuccessfully_WhenNoSceneProvided() {
			var worldBox = new ContextBox<WorldManager>();
			yield return World.CreateContextWithNoSceneProvided(worldBox);
			Assert.That(worldBox.value != null, () => "Failed to create sceneless world");
		}
	}

	[TestFixture]
	public class Serialization {
		[UnityTest]
		public IEnumerator World_SaveAndReload_PersistsBlockChanges() {
			string world = World.CreateNewWorldID();

			var sceneBox = new ContextBox<Scene>();
			var worldBox = new ContextBox<WorldManager>();
			yield return World.CreateSavedContext(worldBox, sceneBox, world);

			WorldDump dump = World.TryGetManager(worldBox).CreateDump();
			ChunkBlockPos pos = new(0, 0, 0);

			pos.chunkX = dump.generatedChunks[0].xpos;
			Block blockAtPos = dump.generatedChunks[0].BlockStateAt(pos).block;
			Block target = Blocks.wood;

			dump.generatedChunks[0].SetBlock(pos, target.defaultState);
			worldBox.value!.SaveWorld(world, dump);

			yield return PlayModeTesting.UnloadSceneAsync(sceneBox.value);

			yield return World.CreateSavedContext(worldBox, sceneBox, world);

			Level level = World.TryGetLevel(worldBox);
			Assert.That(level.BlockAt(pos.ToWorldBlockPos()) == target);
		}

		[UnityTest]
		public IEnumerator World_HasUniqueSaveFile() {
			string world1 = World.CreateNewWorldID();
			string world2 = World.CreateNewWorldID();

			var sceneBox = new ContextBox<Scene>();
			var worldBox = new ContextBox<WorldManager>();
			yield return World.CreateSavedContext(worldBox, sceneBox, world1);
			int world1seed = World.TryGetLevel(worldBox).seed;

			yield return worldBox.value!.TerminateWorldProcess(sceneBox.value, world1,
				() => World.TryGetManager(worldBox).CreateDump());

			yield return World.CreateSavedContext(worldBox, sceneBox, world2);
			int world2seed = World.TryGetLevel(worldBox).seed;

			yield return worldBox.value!.TerminateWorldProcess(sceneBox.value, world2,
				() => World.TryGetManager(worldBox).CreateDump());

			yield return World.CreateSavedContext(worldBox, sceneBox, world1);
			Assert.That(world1seed, Is.EqualTo(World.TryGetLevel(worldBox).seed));

			yield return PlayModeTesting.UnloadSceneAsync(sceneBox.value);

			yield return World.CreateSavedContext(worldBox, sceneBox, world2);
			Assert.That(world2seed, Is.EqualTo(World.TryGetLevel(worldBox).seed));
		}

		[UnityTest]
		public IEnumerator World_LevelInstanceChanges_WhenSwitchingWorlds() {
			string world1 = World.CreateNewWorldID();
			string world2 = World.CreateNewWorldID();
			var worldBox = new ContextBox<WorldManager>();
			var sceneBox = new ContextBox<Scene>();

			yield return World.CreateSavedContext(worldBox, sceneBox, world1);
			Level world1instance = World.TryGetLevel(worldBox);
			worldBox.value!.SaveWorld(world1, World.TryGetManager(worldBox));

			yield return PlayModeTesting.UnloadSceneAsync(sceneBox.value);

			yield return World.CreateSavedContext(worldBox, sceneBox, world2);
			Level world2instance = World.TryGetLevel(worldBox);

			Assert.That(world1instance, Is.Not.EqualTo(world2instance));
		}

		[UnityTest]
		public IEnumerator DoNotSaveWorldStrategy_DiscardsSaves() {
			string world = World.CreateNewWorldID();
			var worldBox = new ContextBox<WorldManager>();

			Scene scene = PlayModeTesting.CreateNewTestScene();
			yield return World.CreateAnonymousContext(scene, worldBox);
			int seed = World.TryGetLevel(worldBox).seed;
			worldBox.value!.SaveWorld(world, World.TryGetManager(worldBox));

			yield return PlayModeTesting.UnloadSceneAsync(scene);

			yield return World.CreateContextWithNoSceneProvided(worldBox, world, new DoNotSaveWorldStrategy());
			Assert.That(seed, Is.Not.EqualTo(World.TryGetLevel(worldBox).seed));
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

			Level level = result.value?.activeLevelManager?.level
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
			worldBox.value!.SaveWorld(world, World.TryGetManager(worldBox));
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
			var worldBox = new ContextBox<WorldManager>();
			yield return World.CreateContextWithNoSceneProvided(worldBox);

			LevelManager levelManager = World.TryGetManager(worldBox);
			ChunkBlockPos pos = new(5, 0, 0);
			pos.chunkX = levelManager.CreateDump().generatedChunks[0].xpos;

			levelManager.level.SetBlock(pos.ToWorldBlockPos(), Blocks.leaves.defaultState);

			var dump = levelManager.CreateDump();
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