using SoulboundBackend.Client.UI.Buttons;
using SoulboundBackend.Client.UI.Containers;
using SoulboundBackend.Client.UI.Frames;
using SoulboundBackend.Client.UI.Layouts;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public sealed class GUI {
		public static GUI instance { get; private set; }
		public static LayoutFactory Layout { get; private set; }
		public static FrameFactory Frame { get; private set; }
		public static ButtonFactory Button { get; private set; }

		public GUI() {
			instance = this;
			Layout = new LayoutFactory();
			Frame = new FrameFactory();
			Button = new ButtonFactory();
		}

		// will be replaced with a ContainerFactory later on
		// for simplicity and implementation limitation reasons, this method should work
		public static ContainerBuilder Container(IUIFrame frame, IUILayoutController layout) {
			return new ContainerBuilder(frame, layout);
		}
	}
}
