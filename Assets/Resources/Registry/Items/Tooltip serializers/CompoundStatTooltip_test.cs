using UnityEngine;

[CreateAssetMenu(fileName = "CompoundStatTooltip_test", menuName = "Scriptable Objects/CompoundStatTooltip_test")]
public class CompoundStatTooltip_test : AbstractTooltipSerializer {

	public override ITooltipSerializer GetSerializer(Item item) {
		return new CustomTooltipData(item);
	}
}

public class CustomTooltipData : ITooltipSerializer {

	private readonly Item item;

	public CustomTooltipData(Item item) => this.item = item;


	public AbstractTooltip Generate() {
		return Tooltip.InterpolatedStats("An item with these stats: {0}, {1}, {2}, {3}", ((IStatProvider)item).Stats);
	}
}