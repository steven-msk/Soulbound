using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;

#nullable enable

public sealed class ArmorItem_test : ArmorItem {
	public override ArmorType armorType => ArmorType.Chestplate;

	public override string name => "armorItem_test";

	public override ItemAspect aspect => _aspect;
	private readonly ItemAspect _aspect = ItemAspect.Simple(new AssetKey("chestplate_overlay"));

	public override int maxStackSize => 1;

	private readonly ModificationToken _token = new();
	public override ModificationToken token => _token;

	public ArmorItem_test() {
			//StatMappingBuilder mappingBuilder = new StatMappingBuilder()
			//	.SetStats(() => new DynamicMap<AbstractValueModifier>() {
			//		["maxHealth"] = new ValueModifier<int>(1, true),
			//		["defense"] = new ValueModifier<int>(10, true)
			//	})
			//	.WithTooltipNodes((stats) => new List<TooltipNodeData>() {
			//		new TooltipNodeData(TooltipNode.Stats, stats["defense"].ToString()),
			//		new TooltipNodeData(TooltipNode.Stats, $"When equipped, gain {stats["maxHealth"]} for 5s")
			//	})
			//	.BindEffectHandlers((stats) => new DynamicMap<IStatEffectHandler>() {
			//		["healthHandler"] = IStatEffectHandler.Timed(5f, false, this, stats["maxHealth"]),
			//		["defenseHandler"] = IStatEffectHandler.Static(this, stats["defense"])
			//	})
			//	.BindActivators((handlers) => new List<StatActivator>() {
			//		new StatActivator(
			//			activationBinder: callback => onEquipped += callback,
			//			deactivationBinder: callback => onUnequipped += callback,
			//			handlers["healthHandler"], handlers["defenseHandler"])
			//	});
			//this.statMappings = mappingBuilder.ResolveMappings();
			//this.tooltipSupplier = (item) => new TooltipData.Builder()
			//	.AddNode(TooltipNode.Title, this.name)
			//	.AddNodes(mappingBuilder.ResolveTooltipNodes())
			//	.Finish();
		}

	public override IEnumerable<StatModificationPackage> GetPackages() {
		throw new NotImplementedException();
	}
}
