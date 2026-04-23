using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.UI.Tooltips {
	public interface ITooltipTrigger {
		void Init(ITooltipRenderer tooltipRenderer);
		void SetTooltip(ITooltip tooltip);
	}
}
