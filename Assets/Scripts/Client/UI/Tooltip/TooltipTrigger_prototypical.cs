using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI {
	public class TooltipTrigger_prototypical : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		private TooltipHandle_prototypical handle;
		private readonly TooltipDefinition tooltip = new("Tooltip");

		public void OnPointerEnter(PointerEventData eventData) {
			handle = Soulbound.instance.GetUIHandler().ShowTooltip(tooltip);
		}

		public void OnPointerExit(PointerEventData eventData) {
			handle?.Destroy();
			handle = null;
		}
	}
}
