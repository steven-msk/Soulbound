using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI.Tooltip {
	public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		public Tooltip tooltip { get; private set; } 

		public void Init(Tooltip tooltip) {
			this.tooltip = tooltip;
		}

		public void OnPointerEnter(PointerEventData eventData) {
			UnityEngine.Debug.Log("show tooltip");
		}

		public void OnPointerExit(PointerEventData eventData) {
			UnityEngine.Debug.Log("hide tooltip");
		}
	}
}
