using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY_INCLUDE_TESTS
public partial class Items {
    internal static ConcurrentDictionary<int, Item> cached = new();

    internal static Item cachedItem_test1 => Lookup("cachedItem_test1", () => new ArmorItem_test());
    internal static Item cachedItem_test2 => Lookup("cachedItem_test2", () => new ConsumableStatItem_test());
    internal static Item cachedItem_test3 => Lookup("cachedItem_test3", () => new StatItem_test());

    internal static TItem Lookup<TItem>(string key, Func<TItem> instanceSupplier) where TItem : Item {
        int hash = key.GetHashCode();
        return (TItem)cached.GetOrAdd(hash, _ => instanceSupplier.Invoke());
    }
}
#endif

public class Items_InstanceTests {

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
}
