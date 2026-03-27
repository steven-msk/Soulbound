using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.UI.Screens {
	[Obsolete]
	public interface IScreenBuilder {
		Screen GetScreen();
	}
}
