using NUnit.Framework;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlockSystem = SoulboundBackend.Client.World.BlockSystem;

public class BlockTests {
    [SetUp]
    public void Setup() {
        _ = typeof(BlockSystem.Blocks).TypeInitializer;
    }

    [OneTimeSetUp]
    public void BootstrapResources() {
        SoulboundBackend.Core.Resource.ResourceGroups.Bootstrap();
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
            int hash = HashHelper.StableHash(attribute.PropertyName);

            var block = BlockSystem.Blocks.ByHashedID(hash);
            Assert.NotNull(block, $"{property.Name} not retrievable by hash fallback");
        }
    }
}
