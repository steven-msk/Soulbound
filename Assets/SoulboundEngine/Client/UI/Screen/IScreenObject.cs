using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Client.UI.Screens;
using SoulboundEngine.Client.UI.Tooltips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Screens {
	public interface IScreenObject : IUIElementContainer, ITooltipManager, ITooltipRenderer {
		void Show();
		void Hide();
		void Dispose();
		Screen GetInstance();
	}
}
