using SoulboundBackend.Client;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class StatItem_test : StatItem {
	public override bool applyInstantStatsOnHoverOrSelect => false;

	public override string name => "StatItem_test";

	public override ItemAspect aspect => ItemAspectRegistry.Get(this, () => ItemAspect.Simple("gem_icon"));

	public override int maxStackSize => Item.CustomMaxStack(128);

	public override IEnumerable<StatMapping> statMappings { get; }

	protected override Func<Item, TooltipData> tooltipSupplier { get; }

	protected override TooltipRenderer.NodeStyleFactory nodeStyleProvider => null;

	public StatItem_test() {
		StatMappingBuilder mappingBuilder = new StatMappingBuilder()
			.SetStats(() => new DynamicMap<AbstractValueModifier>() {
				["physicalDamage"] = new ValueModifier<int>(10, true, StatApplicationType.Percentage)
			})
			.WithTooltipNodes(stats => new List<TooltipNodeData>() {
				new TooltipNodeData(TooltipNode.Stats, $"While in inventory, gain {stats["physicalDamage"]}.")
			})
			.BindEffectHandlers(stats => new DynamicMap<IStatEffectHandler>() {
				["physicalDamageHandler"] = IStatEffectHandler.Static(this, stats["physicalDamage"])
			})
			.BindActivator(handlers => new StatActivator(
				activationBinder: callback => contextHandle.onContextReceived += callback,
				deactivationBinder: callback => contextHandle.onContextLost += callback,
				handlers["physicalDamageHandler"]
			));
		this.statMappings = mappingBuilder.ResolveMappings();
		this.tooltipSupplier = item => new TooltipData.Builder()
			.AddNode(TooltipNode.Title, this.name)
			.AddNodes(mappingBuilder.ResolveTooltipNodes())
			.Finish();
	}

	public override SlotHook GetSlotHook() => new SlotHook(
		onAttached: (itemDisplay, slot) => {
			PlayerController player = Soulbound.instance.GetPlayerInstance();
			if (player.Inventory.GetOccupiedSlots(this).Count() >= 1 && !this.hasContext) {
				//contextHandle.OnContextReceived(player.Stats);
			}
		},
		onDetached: (itemDisplay, slot) => {
			PlayerController player = Soulbound.instance.GetPlayerInstance();
			if (player.Inventory.GetOccupiedSlots(this).Count() == 0) {
				//contextHandle.OnContextLost(player.Stats);
			}
		}
	);
}
