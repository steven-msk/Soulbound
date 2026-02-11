using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public class WorldSessionScreenObject : ScreenObject, IWorldSessionScreenObject {
		private TransitStack transitStack;

		public new void Init(Screen screen) {
			base.Init(screen);
			this.transitStack = new TransitStack(this);
		}

		TransitStack IWorldSessionScreenObject.GetTransitStack() => transitStack;
	}
}
