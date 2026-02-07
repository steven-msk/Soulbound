using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public interface ITooltipComponentBuilder<TBuilder> where TBuilder : ITooltipComponentBuilder<TBuilder> {
		TBuilder Tooltip<TTrigger>(ITooltipDefinition tooltip) where TTrigger : Component, ITooltipTrigger, new();
	}
}
