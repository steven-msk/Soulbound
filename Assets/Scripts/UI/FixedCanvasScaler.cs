using UnityEngine;
using UnityEngine.UI;

public class FixedCanvasScaler : MonoBehaviour {

	public static readonly Vector2 referenceResolution = new(960, 540);
	public static readonly float maxScale = 4.0f;

	private CanvasScaler canvasScaler;

	private void Start() {
		canvasScaler = GetComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
	}

	private void Update() {
		float scaleX = Screen.width / referenceResolution.x;
		float scaleY = Screen.height / referenceResolution.y;

		float scale = Mathf.Max(1, Mathf.Min(scaleX, scaleY));
		canvasScaler.scaleFactor = Mathf.Min(scale, maxScale);
	}
}