using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ConsumableStatItem")]
public class ConsumableStatItem : StatItem, IConsumable {
	[CanBeNull][SerializeField] private ConsumableEffect consumeAction;
	public ConsumableEffect ConsumeAction => consumeAction;

	[SerializeField] private int consumeAmount;
	public int ConsumeAmount => consumeAmount;

	[SerializeField] private List<SerializableStat> stats;
	public override List<SerializableStat> Stats => stats;

	public override bool ApplyStatsAutomatically => false;

	public void Consume(ItemStack itemStack, PlayerController player) {
		ConsumableUtils.DefaultConsume(this, itemStack);
		((IStatProvider)this).ApplyStats(player.Stats);
	}

	protected override CompoundTooltip GetDefaultTooltip() {
		return base.GetDefaultTooltip().Concat(Tooltip.Tag(ItemTag.Consumable));
	}
}
