using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Render.Item {
	public abstract class ItemRenderContext {

		public sealed class GUI : ItemRenderContext {
			public RectTransform? parent;
		}

		public sealed class UIToolkit : ItemRenderContext {
			public const string ITEM_DISPLAY_ELEMENT = "ItemDisplay";
			public const string STACK_COUNT_ELEMENT = "StackCount";
			public VisualElement root;

			public VisualElement GetItemDisplay() { 
				return this.root.Q<VisualElement>(ITEM_DISPLAY_ELEMENT);
			}

			public Label GetStackCount() => this.root.Q<Label>(STACK_COUNT_ELEMENT);
		}

		public sealed class World : ItemRenderContext {
			public Vector3 position;
		}
	}
}
