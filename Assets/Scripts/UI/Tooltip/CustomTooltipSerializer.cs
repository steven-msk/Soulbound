using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Test/CustomTooltip")]
public class CustomTooltipSerializer : AbstractTooltipSerializer {
	public override ITooltipSerializer GetSerializer(Item item) {
		return new CustomTooltipData();
	}
}

public class CustomTooltipData : ITooltipSerializer {
	public AbstractTooltip Generate() {
		return new CustomTooltip(Tooltip.Plain("0").Data);
	}
}

public class CustomTooltip : Tooltip {

	public CustomTooltip(TooltipData data) : base(data) {
	}

	public override void Update(ItemStack itemStack) {
		base.Update(itemStack);
		data.Text = itemStack.Quantity.ToString();
		if (tooltipPanel != null) {
			tooltipPanel.GetComponentInChildren<TextMeshProUGUI>().text = data.Text;
		}
	}
}
