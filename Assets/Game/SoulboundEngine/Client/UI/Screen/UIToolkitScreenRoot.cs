using SoulboundEngine.Client.UI.Screens;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class UIToolkitScreenRoot : IScreenRoot {
		public readonly VisualElement document;

		public UIToolkitScreenRoot(UIDocument document) {
			this.document = document.rootVisualElement;
		}

		public void Attach(VisualElement screenRoot) {
			this.document.Add(screenRoot);
		} 

		void IScreenRoot.AttachScreenObject(GameObject screenObject) {
			throw new NotImplementedException();
		}
	}
}
