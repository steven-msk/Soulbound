using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public abstract class AbstractTooltip {

	// FUTURE TODO: implement interactable tooltips, scrollable/collapsable tooltips
	// PLANNED: tooltip tiling and clamping at screen limits

	[CanBeNull] protected GameObject tooltipPanel;
	[CanBeNull] protected GameObject displayParent;
	
	public GameObject DisplayParent { get => displayParent; set => displayParent = value; }

	public bool IsDisplayed => tooltipPanel != null;

	public abstract void Show(Vector2 position, Transform displayParent);

	public virtual void Hide() {
		GameObject.Destroy(tooltipPanel);
		displayParent = null;
		GameManager.GetPlayerInstance().Inventory.ActiveTooltip = null;
	}

	public virtual void Update(ItemStack itemStack) {
		if (tooltipPanel != null) {
			InputHandler inputHandler = GameManager.GetPlayerInstance().InputHandler;
			tooltipPanel.transform.position = inputHandler.MouseScreenPosition;
		}
	}

	protected static GameObject InstantiatePanel(Transform displayParent) => GameObject.Instantiate(Registry.Get<GameObject>("tooltipPanel"), displayParent, true);

	protected static GameObject InstantiateSection(Transform tooltipPanel) => GameObject.Instantiate(Registry.Get<GameObject>("tooltipSection"), tooltipPanel);

	protected static TextMeshProUGUI InstantiateSectionText(Transform toolipPanel) => InstantiateSection(toolipPanel).GetComponent<TextMeshProUGUI>();
}