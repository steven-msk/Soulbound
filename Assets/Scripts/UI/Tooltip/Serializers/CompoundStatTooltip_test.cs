using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Tooltips/CompoundStatTooltip_test")]
public class CompoundStatTooltip_test : TooltipSerializer {

	public override ITooltipDeserializer GetDeserializer(Item item) {
		return new CustomTooltipData(item);
	}
	private class CustomTooltipData : ITooltipDeserializer {

		private readonly Item item;

		public CustomTooltipData(Item item) => this.item = item;


		public AbstractTooltip Generate() {
			return Tooltip.CompoundStats("An item with these stats", ((IStatProvider)item).instantStats).CompoundConcat(Tooltip.DefaultItem(item))
				.Concat(Tooltip.InterpolatedStats(((StatItemDefinition)item).bufferedInterpolationSource, (((IStatProvider)item).bufferedStats))); 
		}
	}
}
