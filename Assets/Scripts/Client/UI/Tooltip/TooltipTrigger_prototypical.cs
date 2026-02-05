using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI {
	public class TooltipTrigger_prototypical : MonoBehaviour, ITooltipTrigger, IPointerEnterHandler, IPointerExitHandler {
		private ITooltipHandle handle;
		private ITooltipRenderer tooltipRenderer;

		private ITooltipDefinition tooltip = new TooltipDefinition("Tooltip");

		[PROTOTYPICAL]
		private void OnTransformParentChanged() {
			IScreenObject screen = GetComponentInParent<IScreenObject>();
			screen?.AddElement(new UIElementNode(gameObject));
			UnityEngine.Debug.Log("attempting to set parent: " + (screen != null ? screen : "null"));
		}

		void ITooltipTrigger.Init(ITooltipRenderer tooltipRenderer) {
			this.tooltipRenderer = tooltipRenderer;
		}

		void ITooltipTrigger.SetTooltip(ITooltipDefinition tooltip) {
			this.tooltip = tooltip;
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			AssertInit();
			handle = tooltipRenderer.RenderTooltip(tooltip);
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			handle?.Destroy();
			handle = null;
		}

		private void AssertInit() {
			if (tooltipRenderer == null) {
				throw new InvalidOperationException("Tooltip trigger not initialized");
			}
		}
	}
}
