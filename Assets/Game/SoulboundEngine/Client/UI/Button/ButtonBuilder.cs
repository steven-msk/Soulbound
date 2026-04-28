using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Client.UI.Tooltips;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Unity;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Buttons {
	[PROTOTYPICAL]
	public class ButtonBuilder : IUIElementHandleBuilder<IButtonHandle>, ITooltipComponentBuilder<ButtonBuilder> {
		private readonly IUIElementTemplate<LabelButtonHandle> template;
		private string text;
		private bool enabled = true;
		private Action onClick;
		private bool built;
		private ITooltip tooltip;
		private Type tooltipTriggerType;
		private float size = 16f;

		public ButtonBuilder(IUIElementTemplate<LabelButtonHandle> template) {
			this.template = template;
		}

		public ButtonBuilder Text(string text) {
			this.text = text;
			return this;
		}

		public ButtonBuilder Enabled(bool enabled) {
			this.enabled = enabled;
			return this;
		}

		public ButtonBuilder OnClick(Action onClick) {
			this.onClick = onClick;
			return this;
		}

		public ButtonBuilder Size(float size) {
			this.size = size;
			return this;
		}

		public ButtonBuilder Tooltip<TTrigger>(ITooltip tooltip) where TTrigger : Component, ITooltipTrigger, new() {
			if (!typeof(MonoBehaviour).IsAssignableFrom(typeof(TTrigger))) {
				throw new InvalidOperationException("Tooltip trigger component must be a MonoBehaviour");
			}
			this.tooltip = tooltip;
			this.tooltipTriggerType = typeof(TTrigger);
			return this;
		}

		public IButtonHandle Build(IUIElementContainer container) {
			if (this.built) throw new InvalidOperationException("Button already built");
			this.built = true;
			
			GameObject obj = this.template.Instantiate();
			if (this.tooltip != null) {
				ITooltipTrigger trigger = (ITooltipTrigger)obj.AddComponent(this.tooltipTriggerType);
				trigger.SetTooltip(this.tooltip);
			}
			LabelButtonHandle handle = obj.GetOrAddComponent<LabelButtonHandle>();
			handle.Build(this.text, this.enabled, this.onClick, this.size);

			UIElementNode node = new(obj);
			container.AddElement(node);

			this.template.Apply(handle);

			return handle;
		}
	}
}
