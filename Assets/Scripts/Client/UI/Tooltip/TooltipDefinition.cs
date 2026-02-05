using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	// will replace Tooltip and be abstracted
	[PROTOTYPICAL]
	public class TooltipDefinition : ITooltipDefinition {
		private readonly string text;

		public TooltipDefinition(string text) {
			this.text = text;
		}

		public ITooltipHandle Build(ITooltipManager tooltipManager) {
			GameObject obj = new("Tooltip Root", typeof(RectTransform));
			obj.AddComponent<TextMeshProUGUI>().text = text;

			// need a proper handle
			TooltipHandle_prototypical handle = obj.AddComponent<TooltipHandle_prototypical>();

			UITooltipNode node = new(obj, handle);
			tooltipManager.AddTooltip(node);

			return handle;
		}
	}
}
