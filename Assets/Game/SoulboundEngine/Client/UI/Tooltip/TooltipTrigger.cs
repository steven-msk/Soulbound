using SoulboundEngine.Client.UI.Screens;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundEngine.Client.UI.Tooltips {
	public class TooltipTrigger : MonoBehaviour, ITooltipTrigger, IPointerEnterHandler, IPointerExitHandler {
		private ITooltipHandle handle;
		private ITooltipRenderer tooltipRenderer;
		private ITooltip tooltip;

		void ITooltipTrigger.Init(ITooltipRenderer tooltipRenderer) {
			this.tooltipRenderer = tooltipRenderer;
		}

		void ITooltipTrigger.SetTooltip(ITooltip tooltip) {
			this.tooltip = tooltip;
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			AssertInit();
			AssertTooltipNonNull();
			handle = tooltipRenderer.RenderTooltip(tooltip);
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			handle?.Destroy();
			handle = null;
		}

		private void AssertInit() {
			if (tooltipRenderer == null) throw new InvalidOperationException("Tooltip trigger not initialized");
		}

		private void AssertTooltipNonNull() {
			if (tooltip == null) throw new InvalidOperationException("Tooltip not set");
		}
	}
}
