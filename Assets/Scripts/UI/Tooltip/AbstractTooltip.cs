using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public abstract class AbstractTooltip {

	// FUTURE TODO: implement interactable tooltips, scrollable/collapsable tooltips, comparable tooltips for weapons/souls
	// PLANNED: tooltip tiling and clamping at screen limits
	public static float MaxWidth => 850f;
	// POTENTIAL FEATUREIMPL: legendary tooltips

	[CanBeNull] protected GameObject tooltipPanel;
	public GameObject TooltipPanel { get => tooltipPanel; set => tooltipPanel = value; }
	[CanBeNull] protected GameObject displayParent;
	
	public GameObject DisplayParent { get => displayParent; set => displayParent = value; }
	// PLANNED REFACTOR: CompoundTooltip implementation should be integrated in Tooltip to reduce concerns and confusion between tooltip types 

	public bool IsDisplayed => tooltipPanel != null;

	public abstract void Show(Vector2 position, RectTransform displayParent);

	protected float ClampToScreen(RectTransform tooltipPanelRect, Vector2 panelPos) {
		bool isLeftSide = panelPos.x < Screen.width / 2f;
		tooltipPanelRect.pivot = isLeftSide ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
		if (isLeftSide) {
			return Screen.width - panelPos.x;
		} else {
			return panelPos.x;
		}
	}

	public virtual void Hide() {
		GameObject.Destroy(tooltipPanel);
		displayParent = null;
		GameManager.instance.Player.Inventory.ActiveTooltip = null;
	}

	public virtual void Update(ItemStack itemStack) {
		if (tooltipPanel != null) {
			tooltipPanel.transform.position = Input.mousePosition;
		}
	}

	protected static GameObject InstantiatePanel(RectTransform displayParent) => GameObject.Instantiate(Registry.Get<GameObject>("tooltipPanel"), displayParent);

	protected static GameObject InstantiateSection(RectTransform tooltipPanel) => GameObject.Instantiate(Registry.Get<GameObject>("tooltipSection"), tooltipPanel);

	protected static TextMeshProUGUI InstantiateSectionText(RectTransform toolipPanel) => InstantiateSection(toolipPanel).GetComponent<TextMeshProUGUI>();
}
