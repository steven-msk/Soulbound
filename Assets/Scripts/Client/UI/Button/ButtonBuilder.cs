using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Containers;
using SoulboundBackend.Client.UI.Tooltips;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Buttons {
	public class ButtonBuilder : IUIElementHandleBuilder<IButtonHandle>, ITooltipComponentBuilder<ButtonBuilder> {
		private readonly IUIElementTemplate<ButtonHandle> template;
		private string text;
		private bool enabled = true;
		private Action onClick;
		private bool built;
		private ITooltip tooltip;
		private Type tooltipTriggerType;

		public ButtonBuilder(IUIElementTemplate<ButtonHandle> template) {
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

		public ButtonBuilder Tooltip<TTrigger>(ITooltip tooltip) where TTrigger : Component, ITooltipTrigger, new() {
			if (!typeof(MonoBehaviour).IsAssignableFrom(typeof(TTrigger))) {
				throw new InvalidOperationException("Tooltip trigger component must be a MonoBehaviour");
			}
			this.tooltip = tooltip;
			tooltipTriggerType = typeof(TTrigger);
			return this;
		}

		public IButtonHandle Build(IUIElementContainer container) {
			if (built) throw new InvalidOperationException("Button already built");
			built = true;

			GameObject obj = template.Instantiate();
			if (tooltip != null) {
				ITooltipTrigger trigger = (ITooltipTrigger)obj.AddComponent(tooltipTriggerType);
				trigger.SetTooltip(tooltip);
			}
			var handle = obj.GetOrAddComponent<ButtonHandle>();
			handle.Build(text, enabled, onClick);

			UIElementNode node = new(obj);
			container.AddElement(node);

			template.Apply(handle);

			return handle;
		}
	}
}
