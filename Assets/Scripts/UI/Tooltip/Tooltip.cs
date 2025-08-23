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

	protected Tooltip(string text, TooltipSectionLayout layout = null) : this(new TooltipData(layout ?? defaultLayout, text)) {
	}

	protected Tooltip(TooltipData data) => this.data = data;

	public override void Show(Vector2 position, RectTransform parent) {
		if (tooltipPanel != null || data.IsEmpty) {
			return;
		}
		tooltipPanel = AbstractTooltip.InstantiatePanel(parent);
		RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();
		TextMeshProUGUI tooltipSection = AbstractTooltip.InstantiateSectionText(tooltipPanel.GetComponent<RectTransform>());
		panelRect.anchoredPosition = position;
		data.Layout.Apply(tooltipSection);
		tooltipSection.text = data.Text;

		InventoryController inventory = GameManager.instance.Player.Inventory;
		inventory.ActiveTooltip = this;
		tooltipPanel.transform.SetParent(inventory.transform, false);
		tooltipPanel.SetActive(true);
	}

	public static Tooltip NoTooltip() => new("");

	public static Tooltip FromData(TooltipData data) => new(data);

	[CanBeNull] public static Tooltip Info(string text) => !string.IsNullOrEmpty(text) ? new(text) : null;

	public static Tooltip Tag(ItemTag tag) => Tooltip.Tag(tag.ToDisplayString());

	public static Tooltip Tag(string tag) => new(new TooltipData(new TooltipSectionLayout(TooltipSection.Tags), tag)); 

	public static Tooltip Title(string title) => new(title, new TooltipSectionLayout(TooltipSection.Title));

	[CanBeNull] public static Tooltip Lore(string description, TooltipSectionLayout layout = null) => !string.IsNullOrEmpty(description) ? new(description, layout ?? new(TooltipSection.Lore)) : null;

	public static Tooltip Stats(Dictionary<IStatTypeImpl, (object value, bool applyAsBonus)> stats, TooltipSectionLayout layout = null) {
		if (stats.Count == 0) {
			return Tooltip.Info("No stats");
		}
		StringBuilder textBuilder = new();
		stats.OrderBy(statEntry => statEntry.Value.applyAsBonus).ToList().ForEach(statEntry => {
			textBuilder.AppendLine(statEntry.Key.GetFormattedExpression(statEntry.Value.value, statEntry.Value.applyAsBonus));
		});
		if (layout != null && layout.Section != TooltipSection.Stats) {
			UnityEngine.Debug.LogWarning($"Mismatched stat tooltip sections: {layout.Section} and {TooltipSection.Stats}. Switching to {TooltipSection.Stats}.");
			layout.Section = TooltipSection.Stats;
		}
		return new Tooltip(textBuilder.ToString(), layout ?? TooltipSection.Stats.GetDefaultLayout());
	}

	public static Tooltip Stats(IEnumerable<AbstractSerializableStat> stats, TooltipSectionLayout layout = null) {
		return Tooltip.Stats(new Dictionary<IStatTypeImpl, (object, bool)>(stats.OrderBy(stat => stat.GetStatType()).Select(stat => {
			return new KeyValuePair<IStatTypeImpl, (object, bool)>(stat.GetStatType(), (stat.GetBoxedValue(), stat.applyAsBonus));
		})), layout);
	}

	// FUTURE TODO: implement CompoundStats custom header layout
	public static CompoundTooltip CompoundStats(Dictionary<string, IEnumerable<AbstractSerializableStat>> statSections, CompoundTooltipLayout compoundLayout = default) {
		List<TooltipData> data = new(); 
		TooltipSectionLayout commonLayout = TooltipSection.Stats.GetDefaultLayout();

		foreach ((string header, IEnumerable<AbstractSerializableStat> stats) in statSections) {
			List<string> texts = new() { header };
			stats.Select(stat => stat.GetFormattedExpression()).ToList().ForEach(texts.Add);
			data.Add(new TooltipData(commonLayout, string.Join("\n", texts)));
		}
		return CompoundTooltip.OfCustom(compoundLayout, data.ToArray());
	}

	public static CompoundTooltip CompoundStats(string header, IEnumerable<AbstractSerializableStat> stats, CompoundTooltipLayout compoundLayout = default) {
		return Tooltip.CompoundStats(new Dictionary<string, IEnumerable<AbstractSerializableStat>>() { [header] = stats }, compoundLayout);
	}

	[CanBeNull] public static Tooltip InterpolatedStats(string source, params AbstractSerializableStat[] interpolatedStats) {
		if (string.IsNullOrEmpty(source) || interpolatedStats.Count() == 0) {
			return null;
		}
		try {
			return new Tooltip(string.Format(source, interpolatedStats.Select(stat => stat.GetFormattedExpression()).ToArray()), TooltipSection.Stats.GetDefaultLayout());
		} catch (FormatException) {
			UnityEngine.Debug.LogError($"Tooltip source has more stat entries than available; source: '{source}', available entries: {interpolatedStats.Length}");
			return null;
		}
	}

	public static Tooltip InterpolatedStats(string source, IEnumerable<AbstractSerializableStat> interpolatedStats) => InterpolatedStats(source, interpolatedStats.ToArray());

	public static CompoundTooltip InterpolatedStats(string instantSource, IEnumerable<AbstractSerializableStat> interpolatedInstantStats,
			string bufferedSource, IEnumerable<IBufferedStatImpl> interpolatedBufferedStats) {
		return CompoundTooltip.OfNullable(Tooltip.InterpolatedStats(instantSource, interpolatedInstantStats), 
			Tooltip.InterpolatedStats(bufferedSource, interpolatedBufferedStats.Cast<AbstractSerializableStat>()));
	}

	//public static CompoundTooltip DefaultItem(Item item) {
	//	return CompoundTooltip.OfNullable(Tooltip.Title(item.name), Tooltip.Info(item.infoText), Tooltip.Lore(item.loreText));
	//}
}