using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using ItemSystem = SoulboundBackend.Client.ItemSystem;

#if UNITY_INCLUDE_TESTS
public partial class Items {
    internal static ConcurrentDictionary<int, Item> cached = new();

    internal static Item cachedItem_test1 => Lookup("cachedItem_test1", () => new ArmorItem_test());
    internal static Item cachedItem_test2 => Lookup("cachedItem_test2", () => new ConsumableStatItem_test());
    internal static Item cachedItem_test3 => Lookup("cachedItem_test3", () => new StatItem_test());

    internal static TItem Lookup<TItem>(string key, Func<TItem> instanceSupplier) where TItem : Item {
        int hash = key.GetHashCode();
        
        return (TItem)cached.GetOrAdd(hash, hashedID => {
            TItem item = instanceSupplier.Invoke();
            item.hashedID = hashedID;
            return item;
        });
    }
}
#endif

public class ItemTests {

    [OneTimeSetUp]
    public void Setup() {
        SoulboundBackend.Core.Resource.ResourceGroups.Bootstrap();
        StaticResetManager.ResetAll();
    }

    [SetUp]
    public void ItemCleanup() => Items.cached.Clear();
    
    [Test]
    public void ItemInstance_OnlyGetsLoaded_WhenReferenced() {
        Assert.That(!Items.cached.ContainsKey("cachedItem_test1".GetHashCode()));

        Item item = Items.cachedItem_test1;
        Assert.That(item != null);

        Assert.That(Items.cached.Count == 1
            && Items.cached.ToList()
                           .Select(kvp => kvp.Value)
                           .SequenceEqual(new[] { item }));
    }

    [Test]
    public void ItemInstances_RemainUnregistered_UntilReferenced() {
        int hash = "cachedItem_test3".GetHashCode();

        Assert.That(Items.cached.IsEmpty);

        Assert.That(!Items.cached.TryGetValue(hash, out var registered));
        Assert.That(registered == null);

        Item item = Items.cachedItem_test3;
        Assert.That(item != null);

        Assert.That(Items.cached.TryGetValue(hash, out registered));
        Assert.That(registered != null);
    }

    [Test]
    public void Items_SameProperty_ReturnsSameInstance() {
        var item1 = ItemSystem.Items.grassBlock;
        var item2 = ItemSystem.Items.grassBlock;
        Assert.AreSame(item1, item2, "Item cache should return same instance for same property");
    }

    [Test]
    public void Items_ByHashedID_ReturnsSameInstance() {
        int hash = HashHelper.StableHash(nameof(ItemSystem.Items.grassBlock));
        var viaProperty = ItemSystem.Items.grassBlock;
        var viaID = ItemSystem.Items.ByHashedID(hash);
        Assert.AreSame(viaProperty, viaID, "Item retrieved by hash should equal property instance");
    }

    [Test]
    public void Items_ByHashedID_ThrowsForInvalid() {
        int fakeHash = HashHelper.StableHash("nonexistent_item");
        Assert.Throws<KeyNotFoundException>(() => ItemSystem.Items.ByHashedID(fakeHash));
    }

    [Test]
    public void Items_AllProperties_HaveCachedReferences() {
        var staticProperties = typeof(ItemSystem.Items).GetProperties(BindingFlags.Public | BindingFlags.Static);
        foreach (var property in staticProperties) {
            var attribute = property.GetCustomAttribute<ItemSystem.ItemCache>();
            Assert.NotNull(attribute, $"{property.Name} is missing [ItemCache]");
            int hash = HashHelper.StableHash(attribute.PropertyName);

            var item = ItemSystem.Items.ByHashedID(hash);
            Assert.NotNull(item, $"{property.Name} not retreivable by hash fallback");
        }
    }
}
