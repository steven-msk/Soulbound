using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine.Tilemaps;
using BlockSystem = SoulboundBackend.Client.World.BlockSystem;

//public class DummyBlock : BlockSystem.Block {
//	public override string name { get; init; } = "dummy_block";
//	public override TileBase tileReference { get; init; } = null;
//	public override BlockItem itemReference { get; init; } = null;

//	public BlockSystem.BlockProperty<bool> lit;

//	public DummyBlock() : base("dummy_block") {
//	}

//	public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
//		yield return new(Items.cachedItem_test3, 1);
//	}

//	protected override void RegisterProperties(BlockPropertyPool pool) {
//		lit = pool.Register<bool>("lit");
//	}

//	protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
//		return new(this, propertyPool.CreateEntries().With(lit, false));
//	}

//	public override void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
//		registerer.Register(properties.With(lit, true));		
//	}
//}

//namespace BlockTests {
//	[TestFixture]
//	public class InstanceTests {
//		[SetUp]
//		public void Setup() {
//			_ = typeof(BlockSystem.Blocks).TypeInitializer;
//		}

//		[Test]
//		public void RegisterProperties_AddsPropertyToPool() {
//			var block = new DummyBlock();
//			Assert.NotNull(block.lit);
//			Assert.That(block.HasProperty(block.lit));
//		}

//		[Test]
//		public void CreateDefaultState_ReturnsCorrectState() {
//			var block = new DummyBlock();
//			Assert.NotNull(block.defaultState);
//			Assert.That(block.defaultState.Get(block.lit), Is.False);
//		}


//		[Test]
//		public void HashedID_IsSetCorrectly() {
//			var block = new DummyBlock();
//			int hash = HashHelper.StableHash("dummy_block");
//			Assert.AreEqual(hash, block.hashedID);
//		}

//		[Test]
//		public void CreateStates_AddsToPossibleStates() {
//			var block = new DummyBlock();
//			var possibleStates = new[] {
//				block.defaultState.With(block.lit, false),
//				block.defaultState.With(block.lit, true)
//			};
//			Assert.That(block.GetPossibleStates().SequenceEqual(possibleStates));
//		}

//		[Test]
//		public void CreateStates_PostsToRegisty() {
//			var block = new DummyBlock();
//			var state1 = block.defaultState.With(block.lit, false);
//			var state2 = block.defaultState.With(block.lit, true);
//			Assert.That(BlockStateRegistry.Get(state1.stateHash), Is.EqualTo(state1));
//			Assert.That(BlockStateRegistry.Get(state2.stateHash), Is.EqualTo(state2));
//		}

//		[Test]
//		public void DefaultState_IsPostedToRegistry() {
//			var block = new DummyBlock();
//			Assert.That(BlockStateRegistry.Get(block.defaultState.stateHash), Is.EqualTo(block.defaultState));
//		}

//		// the following tests use obsolete features and have been commented out for this reason

//		//[Test]
//		//public void Blocks_SameProperty_ReturnsSameInstance() {
//		//	var block1 = BlockSystem.Blocks.grass;
//		//	var block2 = BlockSystem.Blocks.grass;
//		//	Assert.AreSame(block1, block2, "Block cache should return same instance for same property");
//		//}

//		//[Test]
//		//public void Blocks_ByHashedID_ReturnsSameInstance() {
//		//	int hash = HashHelper.StableHash(nameof(BlockSystem.Blocks.grass));
//		//	var viaProperty = BlockSystem.Blocks.grass;
//		//	var viaID = BlockSystem.Blocks.ByHashedID(hash);
//		//	Assert.AreSame(viaProperty, viaID, "Block retrieved by hash should return equal property instance");
//		//}

//		//[Test]
//		//public void Blocks_ByHashedID_ThrowsForInvalid() {
//		//	int fakeHash = HashHelper.StableHash("nonexistent_block");
//		//	Assert.Throws<KeyNotFoundException>(() => BlockSystem.Blocks.ByHashedID(fakeHash));
//		//}

//		//[Test]
//		//public void Blocks_AllProperties_HaveCachedReferences() {
//		//	var staticProperties = typeof(BlockSystem.Blocks).GetProperties(BindingFlags.Public | BindingFlags.Static);
//		//	foreach (var property in staticProperties) {
//		//		var attribute = property.GetCustomAttribute<BlockSystem.BlockCache>();
//		//		Assert.NotNull(attribute, $"{property.Name} is missing [BlockCache]");
//		//		int hash = HashHelper.StableHash(attribute.propertyName);

//		//		var block = BlockSystem.Blocks.ByHashedID(hash);
//		//		Assert.NotNull(block, $"{property.Name} not retrievable by hash fallback");
//		//	}
//		//}
//	}

//	[TestFixture]
//	public class StateTests {
//		[Test]
//		public void WithProperty_CreatesNewBlockState_WhenPropertyValueDiffers() {
//			var block = new DummyBlock();
//			var state1 = block.defaultState;
//			var state2 = state1.With(block.lit, true);

//			Assert.AreNotSame(state1, state2);
//			Assert.AreNotSame(state1.Get(block.lit), state2.Get(block.lit));
//		}

//		[Test]
//		public void WithProperty_ReturnsSameInstance_WhenValueIsUnchanged() {
//			var block = new DummyBlock();
//			var state1 = block.defaultState;
//			var state2 = state1.With(block.lit, false);

//			Assert.AreSame(state1, state2);
//		}

//		[Test]
//		public void StateHash_ShouldReturnSameInstance() {
//			var block = new DummyBlock();
//			var state = block.defaultState.With(block.lit, false);

//			var hash = BlockStateRegistry.Register(state);
//			Assert.AreEqual(hash, state.stateHash);

//			var deserialized = BlockStateRegistry.Get(hash);

//			Assert.AreEqual(deserialized, state);
//			Assert.AreEqual(deserialized.Get(block.lit), state.Get(block.lit));
//		}
//	}

//}
