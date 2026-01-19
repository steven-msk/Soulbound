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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using Assert = NUnit.Framework.Assert;

//public class MockEntity : Entity, ITickable, IChunkListener, IEntitySpawnable<MockSpawnData> {
//	public override Type scriptType => typeof(MockEntity);
//	public override Facing facing => new Facing(1f);

//	public override EntityDescriptor descriptor => throw new NotImplementedException();

//	public int tickCount = 0;
//	public int loadedCount = 0;
//	public int unloadedCount = 0;
//	public bool applySpawnDataCalled;
//	public MockSpawnData appliedData;

//	void IEntitySpawnable<MockSpawnData>.ApplySpawnData(MockSpawnData spawnData) {
//		applySpawnDataCalled = true;
//		appliedData = spawnData;
//		this.transform.position = spawnData.position;
//	}

//	public void Tick() => tickCount++;
//	public void OnEnteredChunk(WorldChunk chunk) => loadedCount++;
//	public void OnLeftChunk(WorldChunk chunk) => unloadedCount++;

//	private void OnDisable() => Debug.Log("removed");
//	private void OnDestroy() => Debug.Log("removed");
//}

//public class MockSpawnData : ISpawnData {
//	public Vector2 position { get; init; }
//}

//namespace EntitySystemTests {
//	public class EntitySystemTest {
//		internal static GameObject MakePrefabWithEntity<T>() where T : Entity {
//			var go = new GameObject("Prefab_" + typeof(T).Name);
//			go.AddComponent<T>();
//			return go;
//		}
//	}

//	public class MockSubsystem : IEntitySubsystem {
//		public List<Entity> added = new();
//		public void AddEntity(Entity entity) => added.Add(entity);
//		public void RemoveEntity(Entity entity) => added.Remove(entity);
//	}

//	[TestFixture]
//	public class EntityManagementTests : EntitySystemTest {
//		private EntityManager manager;
//		private Level level;

//		[SetUp]
//		public void Setup() {
//			var updater = new GameObject().AddComponent<UpdateManager>();
//			manager = new(new Level(LevelGridContext.FromRuntimePrefabs(), 0), updater);
//		}

//		[Test]
//		public void AddEntity_AssignsID_RegistersSubsystems() {
//			var obj = new GameObject();
//			var entity = obj.AddComponent<MockEntity>();

//			manager.AddEntity(entity);

//			Assert.AreNotEqual(Guid.Empty, entity.id);
//			Assert.AreEqual(manager, entity.manager);
//		}

//		[Test]
//		public void Spawn_CallsApplySpawnData_AndSetsPosition() {
//			var go = new GameObject();
//			var entity = go.AddComponent<MockEntity>();

//			var spawnData = new MockSpawnData() {
//				position = new Vector2(10, 5)
//			};

//			manager.Spawn(entity, spawnData);

//			Assert.AreEqual(new Vector3(10, 5), entity.transform.position);
//			Assert.AreNotEqual(Guid.Empty, entity.id);
//		}

//		[Test]
//		public void Spawn_Descriptor_CreatesEntityAndAssignsId() {
//			var prefab = MakePrefabWithEntity<MockEntity>();
//			var descriptor = new PrefabEntityDescriptor(
//				"entity.test",
//				"Test",
//				"testPrefab",
//				id => prefab
//			);

//			var entity = manager.Spawn(descriptor);

//			Assert.AreNotEqual(Guid.Empty, entity.id);
//		}

//		[Test]
//		public void Spawn_DescriptorWithSpawnData_AppliesSpawnData_AndRegistersEntity() {
//			var prefab = MakePrefabWithEntity<MockEntity>();
//			var descriptor = new PrefabEntityDescriptor(
//				"entity.mock",
//				"Mock",
//				"mockPrefab",
//				id => prefab
//			);
//			var data = new MockSpawnData { position = new Vector2(3, 4) };

//			var entity = manager.Spawn<MockEntity, MockSpawnData>(descriptor, data) as MockEntity;

//			Assert.IsNotNull(entity);
//			Assert.IsTrue(entity.applySpawnDataCalled);
//			Assert.AreEqual((Vector3)data.position, entity.transform.position);
//			Assert.AreNotEqual(Guid.Empty, entity.id);
//		}

//		[Test]
//		public void SpawnSerialized_UsesDescriptor_AndDeserializes() {
//			var prefab = MakePrefabWithEntity<MockEntity>();

//			var descriptor = new PrefabEntityDescriptor(
//				"entity.mock",
//				"Mock",
//				"mockPrefab",
//				id => prefab
//			);

//			typeof(EntityDescriptorRegistry)
//				.GetMethod("Register", BindingFlags.Static | BindingFlags.NonPublic)
//				.Invoke(null, new object[] { descriptor, typeof(MockEntity) });

//			var serialized = new SerializedEntity {
//				id = Guid.NewGuid(),
//				descriptorID = "entity.mock",
//				lastPosition = new Vector2(99, 88),
//				properties = new List<AbstractSerializedEntityProperty>()
//			};

//			var subsystem = new MockSubsystem();
//			manager.AddSubsystem(subsystem);

//			var instance = manager.SpawnSerialized<MockEntity>(serialized);

//			Assert.Contains(instance, subsystem.added);
//			Assert.AreEqual(serialized.id, instance.id);
//		}

//		[Test]
//		public void SpawnSerialized_WithSpawnData_AppliesData_AndDeserializes() {
//			var prefab = MakePrefabWithEntity<MockEntity>();

//			var descriptor = new PrefabEntityDescriptor(
//				"entity.mock2",
//				"Mock2",
//				"mockPrefab2",
//				id => prefab
//			);

//			typeof(EntityDescriptorRegistry)
//				.GetMethod("Register", BindingFlags.Static | BindingFlags.NonPublic)
//				.Invoke(null, new object[] { descriptor, typeof(MockEntity) });

//			var serialized = new SerializedEntity {
//				id = Guid.NewGuid(),
//				descriptorID = "entity.mock2",
//				lastPosition = new Vector2(15, 16),
//				properties = new List<AbstractSerializedEntityProperty>()
//			};

//			var data = new MockSpawnData { position = new Vector2(7, 11) };

//			var subsystem = new MockSubsystem();
//			manager.AddSubsystem(subsystem);

//			var instance = manager.SpawnSerialized<MockEntity, MockSpawnData>(serialized, data);

//			Assert.AreEqual(serialized.id, instance.id);
//			Assert.AreEqual((Vector3)data.position, instance.transform.position);
//			Assert.IsTrue(instance.applySpawnDataCalled);
//			Assert.Contains(instance, subsystem.added);
//		}

//		[Test]
//		public void Spawn_Entity_AssignsId_AndRegisters() {
//			var entity = new GameObject().AddComponent<MockEntity>();

//			manager.Spawn(entity);

//			Assert.AreNotEqual(Guid.Empty, entity.id);
//		}

//		[Test]
//		public void Spawn_MultipleEntities_AssignsUniqueIds() {
//			var e1 = new GameObject().AddComponent<MockEntity>();
//			var e2 = new GameObject().AddComponent<MockEntity>();

//			manager.Spawn(e1);
//			manager.Spawn(e2);

//			Assert.AreNotEqual(e1.id, e2.id);
//		}

//		[Test]
//		public void TickManager_TicksTickables() {
//			var go = new GameObject();
//			var entity = go.AddComponent<MockEntity>();

//			manager.AddEntity(entity);
//			manager.Tick();

//			Assert.AreEqual(1, entity.tickCount);
//		}

//		[UnityTest]
//		public IEnumerator ChunkTracker_OnChunkChange_FiresCallbacks() {
//			var scene = PlayModeTesting.CreateNewTestScene();
//			var worldBox = new ContextBox<WorldManager>();
//			yield return World.CreateContextWithSceneProvided(scene, worldBox);

//			manager = new(worldBox.value.activeLevelManager.level,
//				new GameObject().AddComponent<UpdateManager>());
//			var go = new GameObject();
//			var entity = go.AddComponent<MockEntity>();

//			manager.AddEntity(entity);

//			Assert.AreEqual(1, entity.loadedCount);

//			entity.transform.position = new Vector2(50, 0);

//			manager.Update(0);

//			Assert.AreEqual(1, entity.unloadedCount);
//			yield return PlayModeTesting.UnloadSceneAsync(scene);
//		}

//		[Test]
//		public void RemoveEntity_UnregistersSubsystems() {
//			var go = new GameObject();
//			var entity = go.AddComponent<MockEntity>();

//			manager.AddEntity(entity);

//			LogAssert.Expect("removed");
//			manager.RemoveEntity(entity);
//		}

//	}

//	[TestFixture]
//	public class ComponentSerializationTests {
//		private class TestSerializableComponent : ISerializableComponent {
//			public int health;
//			public string name;

//			public void Save(SerializedEntityPropertyList props) {
//				props.Set("health", health);
//				props.Set("name", name);
//			}

//			public void Read(SerializedEntityPropertyList props) {
//				health = props.Get("health", 0);
//				name = props.Get("name", "");
//			}
//		}

//		[Test]
//		public void Save_WritesComponentFields() {
//			var component = new TestSerializableComponent {
//				health = 40,
//				name = "component"
//			};

//			var list = new SerializedEntityPropertyList();
//			component.Save(list);

//			Assert.IsTrue(list.ContainsKey("health"));
//			Assert.IsTrue(list.ContainsKey("name"));

//			Assert.AreEqual(40, list.Get<int>("health"));
//			Assert.AreEqual("component", list.Get<string>("name"));
//		}

//		[Test]
//		public void Set_OverwritesExistingProperty() {
//			const string key = "key";
//			var list = new SerializedEntityPropertyList();
//			list.Add(key, 10);

//			list.Set(key, 99);

//			Assert.AreEqual(99, list.Get<int>(key));
//		}

//		[Test]
//		public void Get_ReturnsDefault_WhenKeyMissing() {
//			var list = new SerializedEntityPropertyList();

//			int value = list.Get("key", 123);

//			Assert.AreEqual(123, value);
//		}

//		[Test]
//		public void MixedTypes_ArePreservedCorrectly() {
//			var list = new SerializedEntityPropertyList();
//			list.Add("intValue", 10);
//			list.Add("floatValue", 5.5f);
//			list.Add("stringValue", "key");
//			list.Add("vectorValue", new Vector2(2, 3));

//			Assert.AreEqual(10, list.Get<int>("intValue"));
//			Assert.AreEqual(5.5f, list.Get<float>("floatValue"));
//			Assert.AreEqual("key", list.Get<string>("stringValue"));

//			Vector2 vec = list.Get<Vector2>("vectorValue");
//			Assert.AreEqual(new Vector2(2, 3), vec);
//		}

//		[Test]
//		public void PropertyList_CanRoundTripThroughSerializedEntityProperty() {
//			var list = new SerializedEntityPropertyList
//			{
//			new SerializedEntityProperty<int>("hp", 88),
//			new SerializedEntityProperty<string>("tag", "orc")
//		};

//			var rebuilt = SerializedEntityPropertyList.From(list);

//			Assert.AreEqual(88, rebuilt.Get<int>("hp"));
//			Assert.AreEqual("orc", rebuilt.Get<string>("tag"));
//		}

//		[Test]
//		public void ComponentSerialization_RoundTrip_Works() {
//			var component = new TestSerializableComponent {
//				health = 20,
//				name = "name"
//			};

//			var list = new SerializedEntityPropertyList();
//			component.Save(list);

//			var loaded = new TestSerializableComponent();
//			loaded.Read(list);

//			Assert.AreEqual(20, loaded.health);
//			Assert.AreEqual("name", loaded.name);
//		}
//	}

//	[TestFixture]
//	public class EntityDescriptorTests : EntitySystemTest {
//		Dictionary<string, EntityDescriptor> GetById() => 
//			typeof(EntityDescriptorRegistry)
//			   .GetField("byId", BindingFlags.Static | BindingFlags.NonPublic)
//			   .GetValue(null) as Dictionary<string, EntityDescriptor>;

//		Dictionary<Type, EntityDescriptor> GetByType() =>
//			typeof(EntityDescriptorRegistry)
//				.GetField("byType", BindingFlags.Static | BindingFlags.NonPublic)
//				.GetValue(null) as Dictionary<Type, EntityDescriptor>;

//		void ResetRegistry() {
//			GetById().Clear();
//			GetByType().Clear();
//		}

//		[Test]
//		public void CreateInstance_InstantiatesPrefabAndReturnsEntity() {
//			var prefab = MakePrefabWithEntity<MockEntity>();

//			var descriptor = new PrefabEntityDescriptor(
//				id: "entity.test",
//				name: "Test Entity",
//				prefabId: "testPrefab",
//				resourceSelector: id => prefab
//			);

//			Entity instance = descriptor.CreateInstance();

//			Assert.IsNotNull(instance);
//			Assert.IsInstanceOf<MockEntity>(instance);
//			Assert.AreNotSame(prefab, instance.gameObject);
//		}

//		[SetUp]
//		public void Setup() => ResetRegistry();

//		[Test]
//		public void RegisteringDescriptor_StoresByIDAndType() {
//			var prefab = MakePrefabWithEntity<MockEntity>();
//			var descriptor = new PrefabEntityDescriptor(
//				"entity.mock",
//				"Mock Entity",
//				"mockPrefab",
//				id => prefab
//			);

//			typeof(EntityDescriptorRegistry)
//				.GetMethod("Register", BindingFlags.Static | BindingFlags.NonPublic)
//				.Invoke(null, new object[] { descriptor, typeof(MockEntity) });

//			var byId = EntityDescriptorRegistry.ByID("entity.mock");
//			var byType = EntityDescriptorRegistry.ByType(typeof(MockEntity));

//			Assert.AreEqual(descriptor, byId);
//			Assert.AreEqual(descriptor, byType);
//		}

//		[Test]
//		public void ByID_ThrowsIfNotRegistered() {
//			Assert.Throws<KeyNotFoundException>(() => {
//				EntityDescriptorRegistry.ByID("does.not.exist");
//			});
//		}

//		[Test]
//		public void ByType_ThrowsIfNotRegistered() {
//			Assert.Throws<KeyNotFoundException>(() => {
//				EntityDescriptorRegistry.ByType(typeof(MockEntity));
//			});
//		}

//		[Test]
//		public void CreateInstance_CreatesUniqueInstances() {
//			var prefab = MakePrefabWithEntity<MockEntity>();

//			var descriptor = new PrefabEntityDescriptor(
//				"entity.test",
//				"Test Entity",
//				"testPrefab",
//				id => prefab
//			);

//			var e1 = descriptor.CreateInstance();
//			var e2 = descriptor.CreateInstance();

//			Assert.AreNotSame(e1.gameObject, e2.gameObject);
//		}

//		[Test]
//		public void DescriptorStoresNameAndID() {
//			var d = new PrefabEntityDescriptor("entity.test", "TestEntity", "prefab");

//			Assert.AreEqual("entity.test", d.ID);
//			Assert.AreEqual("TestEntity", d.name);
//		}
//	}
//}
