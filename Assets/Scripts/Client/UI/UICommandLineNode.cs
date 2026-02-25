using SoulboundBackend.Core.Debug.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public record UICommandLineNode : UIOverlayNode {
		public readonly ICommandLineHandler handler;

		protected UICommandLineNode(UIOverlayNode original, ICommandLineHandler handler)
			: base(original) {
			this.handler = handler;
		}
	}
}
