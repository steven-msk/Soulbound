using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public bool enableScaleFix = true;
	public static readonly Vector2 referenceResolution = new(960, 540);
	public static readonly float maxScale = 4.0f;

	public Canvas Canvas => this.GetComponent<Canvas>();

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

	public Object InstantiateInUILevel(Object original) {
		return GameObject.Instantiate(original, this.GetRootTransform());
	}

	public RectTransform GetRootTransform() => this.GetComponent<RectTransform>();
}