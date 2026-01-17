using ModestTree;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

#nullable enable

namespace SoulboundBackend.Client.UI {
	[RequireComponent(typeof(Canvas))]
	public class UIManager : ChildReferenceContainer {
		public bool enableScaleFix = true;
		public static readonly Vector2 referenceResolution = new(960, 540);
		public static readonly float maxScale = 4.0f;
		public Screen? activeScreen { get; private set; }
		public ChildReferenceMap screenChildMap { get; } = new();
		private Stack<Screen> screenStack = new();
		private CanvasScaler canvasScaler;

		[Inject] 
		public void Construct(DiContainer container) {
			GetComponent<Canvas>().worldCamera = container.Resolve<Camera>();
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

		public void SetScreen<T>(T? screen, ScreenSetMethod method = ScreenSetMethod.Stack) where T : Screen {
			if (screenStack.TryPeek(out var activeScreen) && method != ScreenSetMethod.Override) {
				activeScreen.OnHide();
			}
			screen?.OnShow();
			if (method == ScreenSetMethod.Stack && screen != null) {
				screenStack.Push(screen);
			}
		}

		public void SetScreen(IScreenBuilder screenBuilder, ScreenSetMethod method = ScreenSetMethod.Stack) {
			this.SetScreen(screenBuilder.GetScreen(), method);
		}

		public void SetScreen<T>(ScreenSetMethod method = ScreenSetMethod.Stack) where T : Screen {
			Screen screen = GameObject.FindFirstObjectByType<T>(FindObjectsInactive.Include);
			if (screen == null) {
				throw new ArgumentException($"Invalid screen type: {typeof(T)}");
			}
			this.SetScreen(screen, method);
		}

		public bool OnEscPressed() {
			if (screenStack.TryPop(out var activeScreen)) {
				activeScreen.OnHide();
				activeScreen.Dispose();
			}

			if (screenStack.TryPeek(out var previousScreen)) {
				SetScreen(previousScreen, ScreenSetMethod.Override);
			}
			return screenStack.IsEmpty();
		}

		public override void RegisterChildReference(ChildReference reference) {
			UnityEngine.Debug.Log("register child in ui manager: " + reference.gameObject.name); 
			if (reference.TryGetComponent<Screen>(out var _)) {
				screenChildMap.RegisterChildReference(reference);
			}
		}

		public Object InstantiateInUILevel(Object original) {
			return GameObject.Instantiate(original, this.GetRootTransform());
		}

		public RectTransform GetRootTransform() => this.GetComponent<RectTransform>();
	}
}