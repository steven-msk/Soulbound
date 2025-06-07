using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class Tooltip : AbstractTooltip {
	private readonly TooltipData data;
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

	public static Tooltip Lore(string description, TooltipSectionLayout layout = null) => new(description, layout ?? new(TooltipSection.Lore));

	public static Tooltip Stats(Dictionary<string, object> stats, string title = null) => Tooltip.Stats(stats, TooltipStatPattern.ValueFirst, title);

	public static Tooltip Stats(Dictionary<string, object> stats, TooltipStatPattern pattern, string title = null, TooltipSectionLayout layout = null) {
		StringBuilder textBuilder = new();
		if (title != null) {
			textBuilder.AppendLine(title);
		}
		stats.ToList().ForEach(stat => textBuilder.AppendLine(pattern.GetPattern(stat)));
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