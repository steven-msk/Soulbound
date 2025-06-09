using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class Tooltip : AbstractTooltip {
	protected readonly TooltipData data;
	public TooltipData Data => data;
	public TooltipSectionLayout Layout => data.Layout;
	private static readonly TooltipSectionLayout defaultLayout = new(TooltipSection.None);
	public TooltipSectionLayout DefaultLayout => defaultLayout;
	public string Text => data.Text;

	private Tooltip(string text, TooltipSectionLayout layout = null) : this(new TooltipData(layout ?? defaultLayout, text)) {
	}

	public Tooltip(TooltipData data) {
		this.data = data;
	}

	public static Tooltip Plain(string text) => new(text);

	public static Tooltip Title(string title) => new(title, new TooltipSectionLayout(TooltipSection.Title));

	[CanBeNull] public static Tooltip Lore(string description, TooltipSectionLayout layout = null) => !string.IsNullOrEmpty(description) ? new(description, layout ?? new(TooltipSection.Lore)) : null;

	public static Tooltip Stats(Dictionary<StatType<float>, object> stats) => Tooltip.Stats(stats);

	public static Tooltip Stats(Dictionary<IStatTypeImpl, object> stats, TooltipSectionLayout layout = null) {
		StringBuilder textBuilder = new();
		stats.ToList().ForEach(stat => {
			IStatTypeImpl statType = stat.Key;
			textBuilder.AppendLine($"{stat.Key.GetFormattedValue(stat.Value)} {statType.GetFormattedName(stat.Value)}");
		});
		if (layout != null && layout.Section != TooltipSection.Stats) {
			Debug.LogWarning($"Mismatched stat tooltip sections: {layout.Section} and {TooltipSection.Stats}. Switching to {TooltipSection.Stats}.");
			layout.Section = TooltipSection.Stats;
		}
		return new Tooltip(textBuilder.ToString(), layout ?? new TooltipSectionLayout(TooltipSection.Stats));
	}

	public static CompoundTooltip Compound(params TooltipData[] entries) => CompoundTooltip.Of(entries);

	public static CompoundTooltip Compound(params Tooltip[] tooltips) => CompoundTooltip.Of(tooltips);

	public override void Show(Vector2 position, Transform parent) {
		if (tooltipPanel != null) {
			return;
		}

		tooltipPanel = AbstractTooltip.InstantiatePanel(parent);
		RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();
		TextMeshProUGUI tooltipSection = AbstractTooltip.InstantiateSectionText(tooltipPanel.transform);
		panelRect.anchoredPosition = position;
		data.Layout.Apply(tooltipSection);
		tooltipSection.text = data.Text;

		InventoryController inventory = GameManager.GetPlayerInstance().Inventory;
		inventory.ActiveTooltip = this;
		tooltipPanel.transform.SetParent(inventory.transform, true);
		tooltipPanel.SetActive(true);
	}
}