using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class ConsumableStatItem_test : ConsumableStatItem {
	public override string name => "ConsumableStatItem_test";

	public override ItemAspect aspect => ItemAspectRegistry.Get(this, () => ItemAspect.Simple("fruit_icon"));

	public override int maxStackSize => Item.DEFAULT_MAX_STACK;

	public override IEnumerable<StatModificationPackage> GetPackages() {
		yield return new(Stats.maxHealth, new[] { new ValueModifier<int>(1, true) });
	}
}
