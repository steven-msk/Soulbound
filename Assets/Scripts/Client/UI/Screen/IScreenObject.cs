using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public interface IScreenObject : IUIElementContainer, ITooltipManager {
		void Show();
		void Hide();
		void Dispose();
		Screen GetInstance();
	}
}
