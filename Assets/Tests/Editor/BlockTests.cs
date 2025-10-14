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

public class DummyBlock : BlockSystem.Block {
    public override string name => "dummy_block";
    public override TileBase tileReference => null;
    public override BlockItem itemReference => null;

    public static readonly BlockSystem.BlockProperty<bool> lit = new("lit");

    protected override void RegisterProperties() {
        propertyMap.Add(lit, false);
    }

    protected override BlockState CreateDefaultState() {
        var defaultProperties = new Dictionary<IBlockStateProperty, object> { { lit, false } };
        return new BlockSystem.BlockState(this, defaultProperties, new DummyBehavior());
    }

    protected override IBlockStateBehavior CreateBehaviorFor(BlockStateProperties properties) {
        bool lit = (bool)properties[DummyBlock.lit];
        return lit ? new LitBehavior() : new DummyBehavior();
    }
}

public class DummyBehavior : IBlockStateBehavior {
    private IBlockStateBehavior inner;
    public bool placed { get; private set; }

    public DummyBehavior() => inner = BlockSystem.CommonBlockBehaviors.DropSingle();

    public List<ItemStack> GetDrops(BlockSystem.BlockState blockState, BreakSource source) {
        return inner.GetDrops(blockState, source);
    }

    public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockSystem.BlockState oldState, BlockSystem.BlockState newState) {
    }

    public void OnPlace(BlockPos blockPos, BlockSystem.BlockState blockState) {
        placed = true;
    }
}

public class LitBehavior : DummyBehavior { }

namespace BlockTests {
    [TestFixture]
    public class InstanceTests {
        [SetUp]
        public void Setup() {
            _ = typeof(BlockSystem.Blocks).TypeInitializer;
        }

        [Test]
        public void Blocks_SameProperty_ReturnsSameInstance() {
            var block1 = BlockSystem.Blocks.grass;
            var block2 = BlockSystem.Blocks.grass;
            Assert.AreSame(block1, block2, "Block cache should return same instance for same property");
        }

        [Test]
        public void Blocks_ByHashedID_ReturnsSameInstance() {
            int hash = HashHelper.StableHash(nameof(BlockSystem.Blocks.grass));
            var viaProperty = BlockSystem.Blocks.grass;
            var viaID = BlockSystem.Blocks.ByHashedID(hash);
            Assert.AreSame(viaProperty, viaID, "Block retrieved by hash should return equal property instance");
        }

        [Test]
        public void Blocks_ByHashedID_ThrowsForInvalid() {
            int fakeHash = HashHelper.StableHash("nonexistent_block");
            Assert.Throws<KeyNotFoundException>(() => BlockSystem.Blocks.ByHashedID(fakeHash));
        }

        [Test]
        public void Blocks_AllProperties_HaveCachedReferences() {
            var staticProperties = typeof(BlockSystem.Blocks).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var property in staticProperties) {
                var attribute = property.GetCustomAttribute<BlockSystem.BlockCache>();
                Assert.NotNull(attribute, $"{property.Name} is missing [BlockCache]");
                int hash = HashHelper.StableHash(attribute.propertyName);

                var block = BlockSystem.Blocks.ByHashedID(hash);
                Assert.NotNull(block, $"{property.Name} not retrievable by hash fallback");
            }
        }
    }

    [TestFixture]
    public class StateTests {
        [Test]
        public void DefaultState_IsRegisteredAndCached() {
            var block = new DummyBlock();
            Assert.NotNull(block.defaultState);
            Assert.AreEqual("dummy_block", block.defaultState.block.name);
        }

        [Test]
        public void Block_HasRegisteredProperty() {
            var block = new DummyBlock();
            Assert.IsTrue(block.HasProperty(DummyBlock.lit));
        }

        [Test]
        public void GetStateFor_CachesIdentical() {
            var block = new DummyBlock();

            var props = new BlockStateProperties(new Dictionary<IBlockStateProperty, object> { { DummyBlock.lit, false } });
            var state1 = block.GetStateFor(props);
            var state2 = block.GetStateFor(props);

            Assert.AreSame(state1, state2);
        }

        [Test]
        public void WithProperty_CreatesNewBlockState_WhenPropertyValueDiffers() {
            var block = new DummyBlock();
            var state1 = block.defaultState;
            var state2 = state1.With(DummyBlock.lit, true);

            Assert.AreNotSame(state1, state2);
            Assert.IsInstanceOf<LitBehavior>(state2.stateBehavior);
        }

        [Test]
        public void WithProperty_ReturnsSameInstance_WhenValueIsUnchanged() {
            var block = new DummyBlock();
            var state1 = block.defaultState;
            var state2 = state1.With(DummyBlock.lit, false);

            Assert.AreSame(state1, state2);
        }

        [Test]
        public void OnPlace_DelegatesToBehavior() {
            var block = new DummyBlock();
            var state = block.defaultState;
            var pos = new BlockPos(0, 0);

            state.OnPlace(pos);

            var stateBehavior = (DummyBehavior)state.stateBehavior;
            Assert.IsTrue(stateBehavior.placed);
        }

        [Test]
        public void BehaviorFactory_ReturnsDifferentBehaviorForDifferentProperties() {
            var block = new DummyBlock();

            var unlitProps = new BlockStateProperties(new Dictionary<IBlockStateProperty, object> { { DummyBlock.lit, false } });
            var litProps = new BlockStateProperties(new Dictionary<IBlockStateProperty, object> { { DummyBlock.lit, true } });

            var unlitState = block.GetStateFor(unlitProps);
            var litState = block.GetStateFor(litProps);

            Assert.IsInstanceOf<DummyBehavior>(unlitState.stateBehavior);
            Assert.IsInstanceOf<LitBehavior>(litState.stateBehavior);
        }

        [Test]
        public void BlockState_EqualityComparer_ConsidersOnlyProperties() {
            var block = new DummyBlock();
            var s1 = block.defaultState;
            var props1 = new BlockStateProperties(new Dictionary<IBlockStateProperty, object> { { DummyBlock.lit, true } });
            var props2 = new BlockStateProperties(new Dictionary<IBlockStateProperty, object> { { DummyBlock.lit, true } });
            var s2 = block.GetStateFor(props1);
            var s3 = block.GetStateFor(props2);

            Assert.AreEqual(props1.GetHashCode(), props2.GetHashCode());

            Assert.IsTrue(s2 == s3);
            Assert.AreEqual(s2.GetHashCode(), s3.GetHashCode());

            Assert.IsTrue(s1 != s2);
            Assert.AreNotEqual(s1.GetHashCode(), s2.GetHashCode());
        }
    }

    [TestFixture]
    public class SerializationTests {
        internal partial class Blocks : ICachedRegistry<Block> {
            [BlockCache(nameof(testBlock))] public static Block testBlock =>
                ICachedRegistry<Block>.Lookup("testBlock", () => new DummyBlock());
        }

        [Test]
        public void Serialize_ShouldReturnValidJArray() {
            var block = new DummyBlock();
            var state = block.defaultState.With(DummyBlock.lit, false);

            JArray result = BlockState.Serializer.Serialize(state);

            Assert.That(result[0]!.Value<int>(), Is.EqualTo(block.hashedID));
            Assert.That(result[1]!.Value<int>(), Is.EqualTo(state.properties.GetHashCode()));
        }

        [Test]
        public void Deserialize_ShouldRestoreBlockState() {
            var block = Blocks.testBlock;
            var state = block.defaultState.With(DummyBlock.lit, false);

            JArray serialized = new JArray { block.hashedID, state.properties.GetHashCode() };
            BlockState deserialized = BlockState.Serializer.Deserialize(serialized);

            Assert.IsTrue(deserialized == state);
            Assert.AreEqual(deserialized.properties, state.properties);
        }

        [Test]
        public void Serialize_ThenDeserialize_ShouldPreserveState() {
            var block = Blocks.testBlock;
            var original = block.defaultState.With(DummyBlock.lit, false);

            var json = BlockState.Serializer.Serialize(original);
            var restored = BlockState.Serializer.Deserialize(json);

            Assert.That(restored.block, Is.EqualTo(original.block));
            Assert.That(restored.properties, Is.EqualTo(original.properties));
        }
    }
}
