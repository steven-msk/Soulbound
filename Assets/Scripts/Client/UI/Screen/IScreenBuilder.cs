using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI.Screens {
	[Obsolete]
	public interface IScreenBuilder {
		Screen GetScreen();
	}
}
