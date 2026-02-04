using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public interface ITooltipDefinition<THandle> where THandle : ITooltipHandle {
		THandle Build(ITooltipManager tooltipManager);
	}
}
