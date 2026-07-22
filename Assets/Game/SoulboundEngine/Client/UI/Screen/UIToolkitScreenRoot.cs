using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class UIToolkitScreenRoot : IScreenRoot {
		public const string ROOT_LAYER_ELEMENT = "RootLayer";
		public readonly VisualElement root;

		public UIToolkitScreenRoot(UIDocument document) {
			this.root = document.rootVisualElement.Q<VisualElement>(ROOT_LAYER_ELEMENT);
		}

		public void Attach(VisualElement screenRoot) {
			this.root.Add(screenRoot);
		} 

		void IScreenRoot.AttachScreenObject(GameObject screenObject) {
			throw new NotImplementedException();
		}
	}
}
