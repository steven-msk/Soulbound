using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public class UIManager : MonoBehaviour {
		public bool enableScaleFix = true;
		public static readonly Vector2 referenceResolution = new(960, 540);
		public static readonly float maxScale = 4.0f;
		public Screen? activeScreen { get; private set; }
		public ChildReferenceMap screenChildMap { get; } = new();
		[SerializeField] private Screen gamePausedScreen;

		public Canvas rootCanvas => this.GetComponent<Canvas>();

		private CanvasScaler canvasScaler;

		private void Awake() {
			BroadcastMessage("OnRegisterChildrenReferences", SendMessageOptions.DontRequireReceiver);
		}

		private void Start() {
			canvasScaler = GetComponent<CanvasScaler>();
		}

		private void Update() {
			if (enableScaleFix) {
				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
				float scaleX = UnityEngine.Screen.width / referenceResolution.x;
				float scaleY = UnityEngine.Screen.height / referenceResolution.y;

				float scale = Mathf.Max(1, Mathf.Min(scaleX, scaleY));
				canvasScaler.scaleFactor = Mathf.Min(scale, maxScale);
			}
		}

		public void SetScreen(Screen? screen) {
			activeScreen?.OnHide();
			activeScreen = screen;
			activeScreen?.OnShow();
		}

		public void SetScreen<T>() where T : Screen {
			Screen screen = GameObject.FindFirstObjectByType<T>(FindObjectsInactive.Include);
			if (screen == null) {
				throw new ArgumentException($"Invalid screen type: {typeof(T)}");
			}
			this.SetScreen(screen);
		}

		public void RegisterChildReference(ChildReference reference) {
			UnityEngine.Debug.Log($"caught child ref: {reference.accessor} @ {this}");
			if (reference.TryGetComponent<Screen>(out var _)) {
				UnityEngine.Debug.Log($"registering screen child: {reference.accessor}");
				screenChildMap.RegisterChildReference(reference);
			}
		}

		public Object InstantiateInUILevel(Object original) {
			return GameObject.Instantiate(original, this.GetRootTransform());
		}

		public RectTransform GetRootTransform() => this.GetComponent<RectTransform>();
	}
}