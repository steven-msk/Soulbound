using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Client.UI.Tooltips;
using System;

namespace SoulboundEngine.Client.UI.Screen {
	[Obsolete]
	public interface IScreenObject : IUIElementContainer, ITooltipManager, ITooltipRenderer {
		void Show();
		void Hide();
		void Dispose();
		Screen GetInstance();
	}
}
