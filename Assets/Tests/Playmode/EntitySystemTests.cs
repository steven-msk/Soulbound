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
using Assert = NUnit.Framework.Assert;

public class MockEntity : Entity, ITickable, IChunkListener, IEntitySpawnable<MockSpawnData> {
	public override Type scriptType => typeof(MockEntity);
	public override Facing facing => new Facing(1f);

	public override EntityDescriptor descriptor => throw new NotImplementedException();

	public int tickCount = 0;
	public int loadedCount = 0;
	public int unloadedCount = 0;

	void IEntitySpawnable<MockSpawnData>.ApplySpawnData(MockSpawnData spawnData) {
		this.transform.position = spawnData.position;
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

		Assert.AreEqual(new Vector2(10, 5), entity.transform.position);
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

[TestFixture]
public class EntityComponentSerializationTests {
	private class TestSerializableComponent : ISerializableComponent {
		public int health;
		public string name;

		public void Save(SerializedEntityPropertyList props) {
			props.Set("health", health);
			props.Set("name", name);
		}

		public void Read(SerializedEntityPropertyList props) {
			health = props.Get("health", 0);
			name = props.Get("name", "");
		}
	}

	[Test]
	public void Save_WritesComponentFields() {
		var component = new TestSerializableComponent {
			health = 40,
			name = "component"
		};

		var list = new SerializedEntityPropertyList();
		component.Save(list);

		Assert.IsTrue(list.ContainsKey("health"));
		Assert.IsTrue(list.ContainsKey("name"));

		Assert.AreEqual(40, list.Get<int>("health"));
		Assert.AreEqual("component", list.Get<string>("name"));
	}

	[Test]
	public void Set_OverwritesExistingProperty() {
		const string key = "key";
		var list = new SerializedEntityPropertyList();
		list.Add(key, 10);

		list.Set(key, 99);

		Assert.AreEqual(99, list.Get<int>(key));
	}

	[Test]
	public void Get_ReturnsDefault_WhenKeyMissing() {
		var list = new SerializedEntityPropertyList();

		int value = list.Get("key", 123);

		Assert.AreEqual(123, value);
	}

	[Test]
	public void MixedTypes_ArePreservedCorrectly() {
		var list = new SerializedEntityPropertyList();
		list.Add("intValue", 10);
		list.Add("floatValue", 5.5f);
		list.Add("stringValue", "key");
		list.Add("vectorValue", new Vector2(2, 3));

		Assert.AreEqual(10, list.Get<int>("intValue"));
		Assert.AreEqual(5.5f, list.Get<float>("floatValue"));
		Assert.AreEqual("key", list.Get<string>("stringValue"));

		Vector2 vec = list.Get<Vector2>("vectorValue");
		Assert.AreEqual(new Vector2(2, 3), vec);
	}

	[Test]
	public void PropertyList_CanRoundTripThroughSerializedEntityProperty() {
		var list = new SerializedEntityPropertyList
		{
			new SerializedEntityProperty<int>("hp", 88),
			new SerializedEntityProperty<string>("tag", "orc")
		};

		var rebuilt = SerializedEntityPropertyList.From(list);

		Assert.AreEqual(88, rebuilt.Get<int>("hp"));
		Assert.AreEqual("orc", rebuilt.Get<string>("tag"));
	}

	[Test]
	public void ComponentSerialization_RoundTrip_Works() {
		var component = new TestSerializableComponent {
			health = 20,
			name = "name"
		};

		var list = new SerializedEntityPropertyList();
		component.Save(list);

		var loaded = new TestSerializableComponent();
		loaded.Read(list);

		Assert.AreEqual(20, loaded.health);
		Assert.AreEqual("name", loaded.name);
	}
}