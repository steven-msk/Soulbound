using SoulboundEngine.UnityClient.UI.UXMLBindings;
using SoulboundEngine.Registry;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.UnityClient.UI.Screen {
	public sealed class UXMLScreenRoot : IScreenRoot {
		private static readonly Identifier ROOT_LAYER_ELEMENT = Identifier.Of("soulbound:screen_root_layer/root_layer");
		public readonly VisualElement root;
		private readonly List<VisualElement> persistentOverlays = new();

		public UXMLScreenRoot(UIDocument document) {
			this.root = document.rootVisualElement.Get<VisualElement>(ROOT_LAYER_ELEMENT);
		}

		public void Attach(VisualElement element) {
			this.root.Add(element);
			foreach (var overlay in this.persistentOverlays) {
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
