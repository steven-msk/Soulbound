namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;

	public abstract class UXMLScreen : Screen {
		protected static readonly UXMLBinding<Label> TOOLTIP_LABEL_ELEMENT = new("soulbound:tooltip/text");
		private readonly VisualTreeAsset asset;
		private readonly List<UXMLWidget> widgets = new();
		private VisualElement root;
		private TemplateContainer tooltip;
		protected IScreenHandle handle;
		public Vector2 mousePos { get; private set; }

		protected UXMLScreen(VisualTreeAsset asset) {
			this.asset = asset;
		}

		protected sealed override void OnBuild(IScreenHandle handle) {
			this.root = handle.Root;
			this.handle = handle;
			this.asset.CloneTree(this.root);
			this.OnBind(this.root);

			this.root.RegisterCallback<MouseMoveEvent>(this.OnMouseMoved, TrickleDown.TrickleDown);
		}

		protected abstract void OnBind(VisualElement root);

		public void AddWidget(UXMLWidget widget) {
			if (this.widgets.Contains(widget)) {
				throw new ArgumentException("Widget is already added");
			}
			widget.SetScreen(this);
			this.widgets.Add(widget);
		}

		public sealed override void SetTooltip(string text) {
			this.ClearTooltip();
			VisualTreeAsset tooltipAsset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("Tooltip"));
			TemplateContainer tooltipRoot = tooltipAsset.Instantiate();
			this.tooltip = tooltipRoot;
			this.root.Add(tooltipRoot);

			Label tooltipLabel = TOOLTIP_LABEL_ELEMENT.Get(tooltipRoot);
			tooltipLabel.text = text;
			this.SetTooltipPosition(this.mousePos);
		}

		public sealed override void ClearTooltip() {
			this.tooltip?.RemoveFromHierarchy();
			this.tooltip = null;
		}

		protected virtual void OnMouseMoved(MouseMoveEvent evt) {
			this.mousePos = evt.mousePosition;
			this.SetTooltipPosition(this.mousePos);
		}

		private void SetTooltipPosition(Vector2 pos) {
			if (this.tooltip == null) return;
			this.tooltip.style.position = Position.Absolute;
			this.tooltip.style.left = pos.x;
			this.tooltip.style.top = pos.y;
		}

		public override bool IsPointerOverUI() => false;

		public override bool HasKeyboardFocus() => false;

		public override void OnDispose(IScreenHandle handle) {
			foreach (UXMLWidget widget in this.widgets) {
				widget.Dispose();
			}
			this.root.UnregisterCallback<MouseMoveEvent>(this.OnMouseMoved, TrickleDown.TrickleDown);
		}
	}
}
