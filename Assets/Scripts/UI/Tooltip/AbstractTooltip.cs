using TMPro;
using UnityEngine;

public abstract class AbstractTooltip {

	//TODO: implement interactable tooltips, scrollable/collapsable tooltips, position relative to mouse screen pos
	protected GameObject tooltipPanel;
	protected GameObject displayParent;
	
	public GameObject DisplayParent { get => displayParent; set => displayParent = value; }

	public bool IsDisplayed => tooltipPanel != null;

	public abstract void Show(Vector2 position, Transform displayParent);

	public virtual void Hide() {
		GameObject.Destroy(tooltipPanel);
		displayParent = null;
		GameManager.GetPlayerInstance().Inventory.ActiveTooltip = null;
	}

	public virtual void Update() {
		if (tooltipPanel != null) {
			InputHandler inputHandler = GameManager.GetPlayerInstance().InputHandler;
			tooltipPanel.transform.position = inputHandler.MouseScreenPosition;
		}
	}

	protected static GameObject InstantiatePanel(Transform displayParent) => GameObject.Instantiate(Registry.Get<GameObject>("tooltipPanel"), displayParent, true);

	protected static GameObject InstantiateSection(Transform tooltipPanel) => GameObject.Instantiate(Registry.Get<GameObject>("tooltipSection"), tooltipPanel);

	protected static TextMeshProUGUI InstantiateSectionText(Transform toolipPanel) => InstantiateSection(toolipPanel).GetComponent<TextMeshProUGUI>();
}