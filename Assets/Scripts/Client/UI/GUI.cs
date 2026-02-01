using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public partial class GUI {
		public static GUI instance { get; private set; }

		public GUI() {
			instance = this;
		}
	}
}
