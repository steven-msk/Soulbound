using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.UI.Buttons {
	public interface IButtonHandle :  IUIElementHandle {
		void SetText(string text);
		void SetEnabled(bool enabled);
		void SetOnClick(Action action);
	}
}
