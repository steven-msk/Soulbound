using Assets.Scripts.Client.UI.Container;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public sealed class GUI {
		public static GUI instance { get; private set; }
		public static ButtonFactory Button { get; private set; }
		public static LayoutFactory Layout { get; private set; }
		public static FrameFactory Frame { get; private set; }

		public GUI() {
			instance = this;
			Button = new ButtonFactory();
			Layout = new LayoutFactory();
			Frame = new FrameFactory();
		}

		public static ContainerBuilder Container() => new();
	}
}
