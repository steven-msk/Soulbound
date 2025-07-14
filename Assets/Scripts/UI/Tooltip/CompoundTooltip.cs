using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CompoundTooltip : AbstractTooltip {
	protected readonly List<TooltipData> entries;
	protected readonly CompoundTooltipLayout layoutOptions;
	protected readonly CompoundTooltipData data;
	public IList<TooltipData> Data => data.tooltips.AsReadOnlyList();

	protected CompoundTooltip(params TooltipData[] entries) : this(default, entries) {
	}

	public CompoundTooltip(CompoundTooltipLayout layout, params TooltipData[] entries) {
		this.layoutOptions = layout ?? new();
		this.entries = entries.ToList();
		data = new CompoundTooltipData(entries, layoutOptions);
	}

	public override void Show(Vector2 position, RectTransform parent) {
		if (tooltipPanel != null) {
			return;
		}
		InventoryController inventory = GameManager.instance.Player.Inventory; 
		tooltipPanel = InstantiatePanel(parent);
		layoutOptions.Apply(tooltipPanel.GetComponent<VerticalLayoutGroup>());
		RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();

		List<TooltipData> sortedEntries = entries.Where(entry => entry.Layout != null).OrderBy(entry => entry.Layout.Section).ToList();
		List<(LayoutElement layoutElement, float preferredWidth)> sectionLayouts = new();
		foreach (TooltipData entry in sortedEntries) {
			TextMeshProUGUI tooltipSection = AbstractTooltip.InstantiateSectionText(tooltipPanel.GetComponent<RectTransform>());
			tooltipSection.textWrappingMode = TextWrappingModes.Normal;
			entry.Layout.Apply(tooltipSection);
			tooltipSection.text = entry.Text;
			sectionLayouts.Add((tooltipSection.GetComponent<LayoutElement>(), tooltipSection.preferredWidth));
		}

		float clampedWidth = Mathf.Min(this.ClampToScreen(panelRect, position), AbstractTooltip.MaxWidth);
		foreach (var (layoutElement, preferredWidth) in sectionLayouts) {
			layoutElement.preferredWidth = Mathf.Min(preferredWidth, clampedWidth);
		}
		panelRect.anchoredPosition = position;
		inventory.ActiveTooltip = this;
		tooltipPanel.transform.SetParent(inventory.transform, false);
		tooltipPanel.SetActive(true);
	}

	public CompoundTooltip Concat(params Tooltip[] tooltips) {
		entries.AddRange(tooltips.Where(tooltip => !tooltip?.Data.IsEmpty ?? tooltip != null).Select(tooltip => tooltip.Data));
		return this;
	}

	public CompoundTooltip CompoundConcat(params CompoundTooltip[] compoundTooltips) {
		entries.AddRange(compoundTooltips.SelectMany(tooltip => tooltip.Data));
		return this;
	}

	public static CompoundTooltip Of(params TooltipData[] entries) => new(entries);

	public static CompoundTooltip Of(params Tooltip[] tooltips) => new(tooltips.Select(tooltip => tooltip.Data).ToArray());

	public static CompoundTooltip OfNullable(params Tooltip[] tooltips) => new(tooltips.Where(tooltip => tooltip != null && !tooltip.Data.IsEmpty).Select(tooltip => tooltip.Data).ToArray());

	public static CompoundTooltip OfCustom(CompoundTooltipLayout layoutOptions, params Tooltip[] tooltips) => new(layoutOptions, tooltips.Where(tooltip => tooltip != null).Select(tooltip => tooltip.Data).ToArray());

	public static CompoundTooltip OfCustom(CompoundTooltipLayout layoutOptions, params TooltipData[] tooltips) => new(layoutOptions, tooltips);
}