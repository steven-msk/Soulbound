using NUnit.Framework;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Core;
using SoulboundBackend.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using WorldTests;
using Assert = UnityEngine.Assertions.Assert;

public class MockEntity : Entity, ITickable, IChunkListener {
	public override Type scriptType => typeof(MockEntity);
	public override string prefabDefinitionID => "mock_entity";
	public override Facing facing => new Facing(1f);

	public int tickCount = 0;
	public int loadedCount = 0;
	public int unloadedCount = 0;

	public override void ApplySpawnData<TData>(TData spawnData) {
		if (spawnData is MockSpawnData sd)
			transform.position = sd.position;
	}

	public void Tick() => tickCount++;
	public void OnEnteredChunk(WorldChunk chunk) => loadedCount++;
	public void OnLeftChunk(WorldChunk chunk) => unloadedCount++;

	private void OnDisable() => Debug.Log("removed");
	private void OnDestroy() => Debug.Log("removed");
}

public class MockSpawnData : ISpawnData {
	public Vector2 position { get; init; }
}

[TestFixture]
public class EntitySystemTests {
	private EntityManager manager;
	private Level level;

	[SetUp]
	public void Setup() {
		var updater = new GameObject().AddComponent<UpdateManager>();
		manager = new(new Level(LevelGridContext.FromRuntimePrefabs(), 0), updater);
	}

	[Test]
	public void AddEntity_AssignsID_RegistersSubsystems() {
		var obj = new GameObject();
		var entity = obj.AddComponent<MockEntity>();

		manager.AddEntity(entity);

		Assert.AreNotEqual(Guid.Empty, entity.id);
		Assert.AreEqual(manager, entity.manager);
	}

	[Test]
	public void Spawn_CallsApplySpawnData_AndSetsPosition() {
		var go = new GameObject();
		var entity = go.AddComponent<MockEntity>();

		var spawnData = new MockSpawnData() {
			position = new Vector2(10, 5)
		};

		manager.Spawn(entity, spawnData);

		Assert.AreEqual<Vector2>(new Vector2(10, 5), entity.transform.position);
		Assert.AreNotEqual(Guid.Empty, entity.id);
	}

	[Test]
	public void TickManager_TicksTickables() {
		var go = new GameObject();
		var entity = go.AddComponent<MockEntity>();

		manager.AddEntity(entity);
		manager.Tick();

		Assert.AreEqual(1, entity.tickCount);
	}

	[UnityTest]
	public IEnumerator ChunkTracker_OnChunkChange_FiresCallbacks() {
		var scene = PlayModeTesting.CreateNewTestScene();
		var worldBox = new ContextBox<WorldManager>();
		yield return World.CreateContextWithSceneProvided(scene, worldBox);

		manager = new(worldBox.value.activeLevelManager.level, 
			new GameObject().AddComponent<UpdateManager>());
		var go = new GameObject();
		var entity = go.AddComponent<MockEntity>();

		manager.AddEntity(entity);

		Assert.AreEqual(1, entity.loadedCount);

		entity.transform.position = new Vector2(50, 0);

		manager.Update(0);

		Assert.AreEqual(1, entity.unloadedCount);
		yield return PlayModeTesting.UnloadSceneAsync(scene);
	}

	[Test]
	public void RemoveEntity_UnregistersSubsystems() {
		var go = new GameObject();
		var entity = go.AddComponent<MockEntity>();

		manager.AddEntity(entity);

		LogAssert.Expect("removed");
		manager.RemoveEntity(entity);
	}

}
