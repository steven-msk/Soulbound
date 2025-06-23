using UnityEngine;

[CreateAssetMenu(menuName = "Items/Custom Tooltips/CompoundStatTooltip_test")]
public class CompoundStatTooltip_test : TooltipSerializer {

	public override ITooltipDeserializer GetDeserializer(Item item) {
		return new CustomTooltipData(item);
	}
}

public class CustomTooltipData : ITooltipDeserializer {

	private readonly Item item;

	public CustomTooltipData(Item item) => this.item = item;


	public AbstractTooltip Generate() {
		return Tooltip.InterpolatedStats("An item with these stats: {0}, {1}, {2}, {3}", ((IStatProvider)item).Stats);
	}
}