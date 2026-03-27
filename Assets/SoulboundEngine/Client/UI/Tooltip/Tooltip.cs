using SoulboundEngine.Common;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Tooltips {
	[PROTOTYPICAL]
	public class Tooltip : ITooltip {
		private readonly string text;

		public Tooltip(string text) {
			this.text = text;
		}

		public ITooltipHandle Build(ITooltipManager tooltipManager) {
			GameObject obj = new("Tooltip Root", typeof(RectTransform));
			obj.AddComponent<TextMeshProUGUI>().text = text;

			// need a proper handle
			TooltipHandle handle = obj.AddComponent<TooltipHandle>();

			UITooltipNode node = new(obj, handle);
			tooltipManager.AddTooltip(node);

			return handle;
		}
	}
}
