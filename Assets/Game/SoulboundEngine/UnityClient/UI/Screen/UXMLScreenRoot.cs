namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;

	public sealed class UXMLScreenRoot : IScreenRoot {
		private static readonly UXMLBinding<VisualElement> ROOT_LAYER_ELEMENT = new("soulbound:screen_root_layer/root_layer");
		public readonly VisualElement root;
		private readonly List<VisualElement> persistentOverlays = new();

		public UXMLScreenRoot(UIDocument document) {
			this.root = ROOT_LAYER_ELEMENT.Get(document.rootVisualElement);
		}

		public void Attach(VisualElement element) {
			this.root.Add(element);
			foreach (VisualElement overlay in this.persistentOverlays) {
				overlay.BringToFront();
			}
		}

		public void AttachPersistentOverlay(VisualElement element) {
			this.persistentOverlays.Add(element);
			this.Attach(element);
		}

		void IScreenRoot.AttachScreenObject(GameObject screenObject) {
			throw new NotImplementedException();
		}
	}
}
