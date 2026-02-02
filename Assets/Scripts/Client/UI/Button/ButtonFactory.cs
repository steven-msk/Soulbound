using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public sealed class ButtonFactory {
		public ButtonBuilder FromTemplate(IUIElementTemplate<ButtonHandle> template) {
			return new ButtonBuilder(template);
		}

		public ButtonBuilder Label() => FromTemplate(new LabelButton());
	}
}
