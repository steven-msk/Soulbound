using SoulboundBackend.Common;
using System;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.UI.Tooltip {
	public class Tooltip {
		private static readonly Logger logger = Logger.CreateInstance();
		// FUTURE TODO: implement interactable tooltips, scrollable/collapsable tooltips, comparable tooltips for weapons/souls
		// PLANNED: tooltip tiling and clamping at screen limits
		public static float MaxWidth => 850f;
		// POTENTIAL FEATUREIMPL: legendary tooltips
		public TooltipData? data { get; private set; }
		private TooltipRenderer renderer;
		private GameObject? panel;

		public bool IsDisplaying => panel != null;

		public Tooltip(TooltipRenderer renderer, TooltipData data) {
			this.data = data;
			this.renderer = renderer;
		}

		public void Show(Vector2 position, Transform parent) {
			if (data == null) {
				return;
			}
			panel = renderer.Render(data, position, parent);
			panel.SetActive(true);
		}

		public void Hide() {
			if (panel != null) {
				GameObject.Destroy(panel);
			}
		}

		public void SetParent(Transform parent, bool worldPositionStays = false) {
			if (data == null) {
				return;
			}
			this.panel.NullOrElse((panel) => {
			}, () => logger.LogWarning("Discarded attempt to set parent to null tooltip panel"));
		}

		public void SetPosition(Vector2 screenPosition) {
			if (data == null) {
				return;
			}
			this.panel.NullOrElse((panel) => {
				panel.transform.position = screenPosition;
			}, () => logger.LogWarning("Discarded attempt to set position to null tooltip panel"));
		}

		public static TooltipData Plain(string text) => new TooltipData.Builder().AddNode(TooltipNode.None, text).Finish();

		public static Tooltip NoTooltip() => new(null!, null!);

		//public virtual void Update(ItemStack itemStack) {
		//	if (tooltipPanel != null) {
		//		tooltipPanel.transform.position = Input.mousePosition;
		//	}
		//}

		//protected float ClampToScreen(RectTransform tooltipPanelRect, Vector2 panelPos) {
		//	bool isLeftSide = panelPos.x < Screen.width / 2f;
		//	tooltipPanelRect.pivot = isLeftSide ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
		//	if (isLeftSide) {
		//		return Screen.width - panelPos.x;
		//	} else {
		//		return panelPos.x;
		//	}
		//}

		//public static Tooltip NoTooltip() => new("");

		//public static Tooltip FromData(TooltipNodeData data) => new(data);

		//[CanBeNull] public static Tooltip Info(string text) => !string.IsNullOrEmpty(text) ? new(text) : null;

		//public static Tooltip Tag(ItemTag tag) => Tooltip.Tag(tag.ToDisplayString());

		//public static Tooltip Tag(string tag) => new(new TooltipData(new TooltipNodeStyle(TooltipNode.Tags), tag)); 

		//public static Tooltip Title(string title) => new(title, new TooltipNodeStyle(TooltipNode.Title));

		//[CanBeNull] public static Tooltip Lore(string description, TooltipNodeStyle layout = null) => !string.IsNullOrEmpty(description) ? new(description, layout ?? new(TooltipNode.Lore)) : null;

		//public static Tooltip Stats(Dictionary<IStatDefinitionImpl, (object value, bool applyAsBonus)> stats, TooltipNodeStyle layout = null) {
		//	if (stats.Count == 0) {
		//		return Tooltip.Info("No stats");
		//	}
		//	StringBuilder textBuilder = new();
		//	stats.OrderBy(statEntry => statEntry.Value.applyAsBonus).ToList().ForEach(statEntry => {
		//		textBuilder.AppendLine(statEntry.Key.GetFormattedExpression(statEntry.Value.value, statEntry.Value.applyAsBonus));
		//	});
		//	if (layout != null && layout.Section != TooltipNode.Stats) {
		//		UnityEngine.Debug.LogWarning($"Mismatched stat tooltip sections: {layout.Section} and {TooltipNode.Stats}. Switching to {TooltipNode.Stats}.");
		//		layout.Section = TooltipNode.Stats;
		//	}
		//	return new Tooltip(textBuilder.ToString(), layout ?? TooltipNode.Stats.GetDefaultLayout());
		//}

		//public static Tooltip Stats(IEnumerable<AbstractSerializableStat> stats, TooltipNodeStyle layout = null) {
		//	return Tooltip.Stats(new Dictionary<IStatDefinitionImpl, (object, bool)>(stats.OrderBy(stat => stat.GetStatDefinition()).Select(stat => {
		//		return new KeyValuePair<IStatDefinitionImpl, (object, bool)>(stat.GetStatDefinition(), (stat.GetBoxedValue(), stat.applyAsBonus));
		//	})), layout);
		//}

		//public static CompoundTooltip CompoundStats(Dictionary<string, IEnumerable<AbstractSerializableStat>> statSections, CompoundTooltipLayout compoundLayout = default) {
		//	List<TooltipNodeData> data = new(); 
		//	TooltipNodeStyle commonLayout = TooltipNode.Stats.GetDefaultLayout();

		//	foreach ((string header, IEnumerable<AbstractSerializableStat> stats) in statSections) {
		//		List<string> texts = new() { header };
		//		stats.Select(stat => stat.GetFormattedExpression()).ToList().ForEach(texts.Add);
		//		data.Add(new TooltipData(commonLayout, string.Join("\n", texts)));
		//	}
		//	return CompoundTooltip.OfCustom(compoundLayout, data.ToArray());
		//}

		//public static CompoundTooltip CompoundStats(string header, IEnumerable<AbstractSerializableStat> stats, CompoundTooltipLayout compoundLayout = default) {
		//	return Tooltip.CompoundStats(new Dictionary<string, IEnumerable<AbstractSerializableStat>>() { [header] = stats }, compoundLayout);
		//}

		//[CanBeNull] public static Tooltip InterpolatedStats(string source, params AbstractSerializableStat[] interpolatedStats) {
		//	if (string.IsNullOrEmpty(source) || interpolatedStats.Count() == 0) {
		//		return null;
		//	}
		//	try {
		//		return new Tooltip(string.Format(source, interpolatedStats.Select(stat => stat.GetFormattedExpression()).ToArray()), TooltipNode.Stats.GetDefaultLayout());
		//	} catch (FormatException) {
		//		UnityEngine.Debug.LogError($"Tooltip source has more stat entries than available; source: '{source}', available entries: {interpolatedStats.Length}");
		//		return null;
		//	}
		//}

		//public static Tooltip InterpolatedStats(string source, IEnumerable<AbstractSerializableStat> interpolatedStats) => InterpolatedStats(source, interpolatedStats.ToArray());

		//public static CompoundTooltip InterpolatedStats(string instantSource, IEnumerable<AbstractSerializableStat> interpolatedInstantStats,
		//		string bufferedSource, IEnumerable<IBufferedStatImpl> interpolatedBufferedStats) {
		//	return CompoundTooltip.OfNullable(Tooltip.InterpolatedStats(instantSource, interpolatedInstantStats), 
		//		Tooltip.InterpolatedStats(bufferedSource, interpolatedBufferedStats.Cast<AbstractSerializableStat>()));
		//}
	}
}