using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Tooltips {
	public interface ITooltipComponentBuilder<TBuilder> where TBuilder : ITooltipComponentBuilder<TBuilder> {
		TBuilder Tooltip<TTrigger>(ITooltip tooltip) where TTrigger : Component, ITooltipTrigger, new();
	}
}
