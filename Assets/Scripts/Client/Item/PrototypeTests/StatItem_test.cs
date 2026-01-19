using SoulboundBackend.Client;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class StatItem_test : StatItem {
	public override string name => "StatItem_test";

	public override ItemAspect aspect => ItemAspectRegistry.Get(this, () => ItemAspect.Simple(new AssetKey("gem_icon")));

	public override int maxStackSize => Item.CustomMaxStack(128);

	private readonly ModificationToken _token = new();
	public override ModificationToken token => _token;
	private bool appliedStats = false;

	public override void OnAttachedInSlot(IItemSlot slot) {
		if (slot.container is IStatContextProvider statContext && !appliedStats) {
			statContext.statModificationHost.ApplyModifiers(this);
			this.appliedStats = true;
		}
	}

	public override void OnDetachedFromSlot(IItemSlot slot) {
		if (slot.container.ContainsItem(this)) {
			return;
		}
		if (slot.container is IStatContextProvider statContext) {
			statContext.statModificationHost.RemoveModifiers(this);
			this.appliedStats = false;
		}
	}

	public override IEnumerable<StatModificationPackage> GetPackages() {
		yield return new(Stats.critChance, new[] { new ValueModifier<float>(10f, true, StatApplicationType.Percentage) });
	}
}
