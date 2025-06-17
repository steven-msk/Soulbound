using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ConsumableStatItem")]
public class ConsumableStatItem : Item, IConsumable, IStatProvider {
	[CanBeNull][SerializeField] private ConsumableEffect consumeAction;
	public ConsumableEffect ConsumeAction => consumeAction;

	[SerializeField] private int consumeAmount;
	public int ConsumeAmount => consumeAmount;

	[SerializeField] private List<SerializableStat> stats;
	public List<SerializableStat> Stats => stats;

	public bool ApplyStatsAutomatically => false;

	public void Consume(ItemStack itemStack, PlayerController player) {
		ConsumableUtils.DefaultConsume(this, itemStack, player);
		((IStatProvider)this).ApplyStats(player);
	}

	protected override AbstractTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(TooltipData.Concat((base.GetDefaultTooltip() as CompoundTooltip).Data.ToArray(),
			new TooltipData[] { Tooltip.Stats(stats).Data, Tooltip.Tag("Consumable").Data }));
	}
}
