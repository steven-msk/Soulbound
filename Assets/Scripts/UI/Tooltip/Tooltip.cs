using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class Tooltip : AbstractTooltip {
	protected readonly TooltipData data;
	public TooltipData Data => data;
	public TooltipSectionLayout Layout => data.Layout;
	private static readonly TooltipSectionLayout defaultLayout = new(TooltipSection.Info);
	public TooltipSectionLayout DefaultLayout => defaultLayout;
	public string Text => data.Text;

	private Tooltip(string text, TooltipSectionLayout layout = null) : this(new TooltipData(layout ?? defaultLayout, text)) {
	}

	public Tooltip(TooltipData data) {
		this.data = data;
	}

	[CanBeNull] public static Tooltip Info(string text) => !string.IsNullOrEmpty(text) ? new(text) : null;

	public static Tooltip Title(string title) => new(title, new TooltipSectionLayout(TooltipSection.Title));

	[CanBeNull] public static Tooltip Lore(string description, TooltipSectionLayout layout = null) => !string.IsNullOrEmpty(description) ? new(description, layout ?? new(TooltipSection.Lore)) : null;

	public static Tooltip Stats(Dictionary<IStatTypeImpl, object> stats, TooltipSectionLayout layout = null, bool applyAsBonus = false) {
		if (stats.Count == 0) {
			return Tooltip.Info("No stats");
		}
		
		StringBuilder textBuilder = new();
		stats.ToList().ForEach(stat => {
			textBuilder.AppendLine($"{stat.Key.GetFormattedValue(stat.Value, applyAsBonus)} {stat.Key.GetFormattedName(stat.Value)}");
		});
		if (layout != null && layout.Section != TooltipSection.Stats) {
			Debug.LogWarning($"Mismatched stat tooltip sections: {layout.Section} and {TooltipSection.Stats}. Switching to {TooltipSection.Stats}.");
			layout.Section = TooltipSection.Stats;
		}
		return new Tooltip(textBuilder.ToString(), layout ?? new TooltipSectionLayout(TooltipSection.Stats));
	}

	public static Tooltip Stats(IEnumerable<SerializableStat> stats, TooltipSectionLayout layout = null, bool applyAsBonus = false) {
		return Tooltip.Stats(new Dictionary<IStatTypeImpl, object>(stats.Select(stat => {
			return new KeyValuePair<IStatTypeImpl, object>(stat.serializedReference.ToStatType(), stat.GetValue());
		})), layout, applyAsBonus);
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