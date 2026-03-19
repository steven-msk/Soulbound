using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.UI.Tooltips {
	public interface ITooltipHandle {
		event Action? onDestroyed;
		bool isAlive { get; }
		void Destroy();
	}
}
