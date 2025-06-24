using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultStatsTooltip", menuName = "Tooltips/DefaultStatsTooltip")]
public class DefaultStatsTooltip : TooltipSerializer {

	public override ITooltipDeserializer GetDeserializer(Item item) => new StatsTooltipData(item);

	private class StatsTooltipData : ITooltipDeserializer {

		private Item item;

		public StatsTooltipData(Item item) => this.item = item;

		public AbstractTooltip Generate() => Tooltip.Default(item).Concat(Tooltip.Stats(((IStatProvider)item).Stats));
	}
}


