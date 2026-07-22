using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Core.Registry;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Render.Item {
	public abstract class ItemRenderContext {

		public sealed class GUI : ItemRenderContext {
			public RectTransform? parent;
		}

		public sealed class UIToolkit : ItemRenderContext {
			public readonly Identifier itemDisplayElement;
			public readonly Identifier stackCountElement;
			public readonly VisualElement root;

			public UIToolkit(VisualElement root, Identifier itemDisplayElement, Identifier stackCountElement) {
				this.root = root;
				this.itemDisplayElement = itemDisplayElement;
				this.stackCountElement = stackCountElement;
			}

			public VisualElement GetItemDisplay() { 
				return this.root.Get<VisualElement>(this.itemDisplayElement);
			}

			public Label GetStackCount() => this.root.Get<Label>(this.stackCountElement);
		}

		public sealed class World : ItemRenderContext {
			public Vector3 position;
		}
	}
}
