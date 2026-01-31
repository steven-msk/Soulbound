using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public interface IScreenObject {
		void Show();
		void Hide();
		Screen GetInstance();
	}
}
