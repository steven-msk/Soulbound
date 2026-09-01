namespace SoulboundEngine.UnityClient.Render.Item {
	using SoulboundEngine.Common.Math;
	using System;
	using UnityEngine;
	using UnityEngine.UIElements;

#nullable enable

	public abstract class ItemRenderContext {

		public sealed class UGUI : ItemRenderContext {
			public RectTransform? parent;
		}

		public sealed class UXML : ItemRenderContext {
			private readonly VisualElement root;
			private readonly Func<VisualElement, VisualElement> displayElementSupplier;
			private readonly Func<VisualElement, Label> countElementSupplier;

			public UXML(VisualElement root, Func<VisualElement, VisualElement> displayElementSupplier, Func<VisualElement, Label> countElementSupplier) {
				this.root = root;
				this.displayElementSupplier = displayElementSupplier;
				this.countElementSupplier = countElementSupplier;
			}

			public VisualElement GetItemDisplay() => this.displayElementSupplier(this.root);

			public Label GetStackCount() => this.countElementSupplier(this.root);

			public VisualElement GetRoot() => this.root;

			public void SetVisible(VisualElement visualElement, bool visible) {
				visualElement.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
			}
		}

		public sealed class World : ItemRenderContext {
			public Vec2d position;
		}
	}
}
