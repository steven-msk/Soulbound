using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	[Header("Flight Time Panel")]
	[SerializeField] private GameObject flightTimePanel;
	[SerializeField] private float maskWidth;               // maskWidth must be equal to TimeBar width (center&middle anchor)
	public bool enableScaleFix = true;
	
	public void UpdateFlightTimePanel(bool isFlying, float flightTime, float grantedFlightTime) {
		flightTimePanel.SetActive(isFlying && GameManager.GetPlayerInstance().InputHandler.PressingSpace);
		RectMask2D timeMask = flightTimePanel.GetComponentInChildren(typeof(RectMask2D), true) as RectMask2D;
		timeMask.padding = new Vector4(maskWidth * (1 - flightTime / grantedFlightTime), 0, 0, 0);
	}

	public static readonly Vector2 referenceResolution = new(960, 540);
	public static readonly float maxScale = 4.0f;

	private CanvasScaler canvasScaler;

	private void Start() {
		canvasScaler = GetComponent<CanvasScaler>();
	}

	private void Update() {
		if (enableScaleFix) {
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
			float scaleX = Screen.width / referenceResolution.x;
			float scaleY = Screen.height / referenceResolution.y;

			float scale = Mathf.Max(1, Mathf.Min(scaleX, scaleY));
			canvasScaler.scaleFactor = Mathf.Min(scale, maxScale);
		}
	}
}