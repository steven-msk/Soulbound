using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class UIToolkitScreenRoot : IScreenRoot {
		private static readonly Identifier ROOT_LAYER_ELEMENT = Identifier.Of("soulbound:screen_root_layer/root_layer");
		public readonly VisualElement root;

		public UIToolkitScreenRoot(UIDocument document) {
			this.root = document.rootVisualElement.Get<VisualElement>(ROOT_LAYER_ELEMENT);
		}

		public void Attach(VisualElement screenRoot) {
			this.root.Add(screenRoot);
		} 

		void IScreenRoot.AttachScreenObject(GameObject screenObject) {
			throw new NotImplementedException();
		}
	}
}
