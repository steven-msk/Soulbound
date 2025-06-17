using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
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

	public static Tooltip Tag(string tagName) => new(new TooltipData(new TooltipSectionLayout(TooltipSection.Tags), tagName));

	public static Tooltip Title(string title) => new(title, new TooltipSectionLayout(TooltipSection.Title));

	[CanBeNull] public static Tooltip Lore(string description, TooltipSectionLayout layout = null) => !string.IsNullOrEmpty(description) ? new(description, layout ?? new(TooltipSection.Lore)) : null;

#warning TODO implement a way to accept both bonus and static stat values on one item - stat tooltips may contain both bonus and static values, along with headers to describe their purpose or appliance rules.
	// maybe add a special class PredicateStat for predicate-based stat appliances - name is subject to change

	// Dictionary<IStatTypeImpl, (object value, bool applyAsValue)> adapted dictionary option 
	// or remove Dictionary implementation and rely only on IEnumerable<SerializableStat>
	public static Tooltip Stats(Dictionary<IStatTypeImpl, (object value, bool applyAsBonus)> stats, TooltipSectionLayout layout = null) {
		if (stats.Count == 0) {
			return Tooltip.Info("No stats");
		}
		
		StringBuilder textBuilder = new();
		stats.OrderBy(statEntry => statEntry.Value.applyAsBonus).ToList().ForEach(statEntry => {
			textBuilder.AppendLine(statEntry.Key.GetFormattedExpression(statEntry.Value.value, statEntry.Value.applyAsBonus));
		});
		if (layout != null && layout.Section != TooltipSection.Stats) {
			Debug.LogWarning($"Mismatched stat tooltip sections: {layout.Section} and {TooltipSection.Stats}. Switching to {TooltipSection.Stats}.");
			layout.Section = TooltipSection.Stats;
		}
		return new Tooltip(textBuilder.ToString(), layout ?? new TooltipSectionLayout(TooltipSection.Stats));
	}

	public static Tooltip Stats(IEnumerable<SerializableStat> stats, TooltipSectionLayout layout = null) {
		return Tooltip.Stats(new Dictionary<IStatTypeImpl, (object, bool)>(stats.OrderBy(stat => stat.SerializedReference).Select(stat => {
			return new KeyValuePair<IStatTypeImpl, (object, bool)>(stat.SerializedReference.ToStatType(), (stat.GetValue(), stat.ApplyAsBonus));
		})), layout);
	}

	public static CompoundTooltip CompoundStats(Dictionary<string, IEnumerable<SerializableStat>> statSections) {
		List<TooltipData> data = new();
		TooltipSectionLayout layout = new(TooltipSection.Stats);

		foreach ((string header, IEnumerable<SerializableStat> stats) in statSections) {
			List<string> texts = new() { header };
			stats.Select(stat => stat.GetFormattedExpression()).ToList().ForEach(texts.Add);
			data.Add(new TooltipData(layout, string.Join("\n", texts)));
		}
		return CompoundTooltip.Of(data.ToArray());
	}

	public static Tooltip InterpolatedStats(string source, IEnumerable<SerializableStat> interpolatedStats) {
		return new Tooltip(string.Format(source, interpolatedStats.Select(stat => stat.GetFormattedExpression()).ToArray()), new TooltipSectionLayout(TooltipSection.Stats));
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