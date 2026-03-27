using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.UI.Screens {
	public interface IScreenNavigator {
		void PushScreen(Screen screen);
		void ReplaceScreen(Screen screen);
		bool PopScreen();
	}
}
