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

namespace Assets.Scripts.Client.UI.Tooltip {
	// will be replaced by Tooltip and abstracted
	[PROTOTYPICAL]
	public class TooltipDefinition : ITooltipDefinition<ITooltipHandle> {
		private readonly string text;

		public TooltipDefinition(string text) {
			this.text = text;
		}

		public ITooltipHandle Build(ITooltipManager tooltipManager) {
			GameObject obj = new("Tooltip Root", typeof(RectTransform));
			obj.AddComponent<TextMeshProUGUI>().text = text;

			ITooltipHandle handle = default;

			UITooltipNode node = new(obj, handle);
			tooltipManager.AddTooltip(node);

			return handle;
		}
	}
}
