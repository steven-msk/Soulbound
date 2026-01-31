using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI.Screens {
	public interface IScreenObject {
		void Show();
		void Hide();
		void Dispose();
		Screen GetInstance();
	}
}
