using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Render.Item {
	public abstract class ItemRenderContext {

		public sealed class GUI : ItemRenderContext {
			public RectTransform? parent;
		}

		public sealed class UIToolkit : ItemRenderContext {
			public VisualElement root;

			public VisualElement GetItemDisplay() => this.root.Q<VisualElement>("ItemDisplay");
			public Label GetStackCount() => this.root.Q<Label>("StackCount");
		}

		public sealed class World : ItemRenderContext {
			public Vector3 position;
		}
	}
}
