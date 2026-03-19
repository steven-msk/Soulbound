using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI.Tooltips {
	public interface ITooltipRenderer {
		ITooltipHandle RenderTooltip(ITooltip tooltip);
	}
}
