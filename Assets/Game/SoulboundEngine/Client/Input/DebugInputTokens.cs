using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Input {
	public static partial class InputTokens {
		public static class Debug {
			public static InputToken toggleConsole;
			public static InputToken toggleMetrics;
			public static InputToken enterCommand;

			public static void Register(InputActionAsset asset) {
				toggleConsole = Create(asset, "Debug/ToggleConsole");
				enterCommand = Create(asset, "Debug/EnterCommand");
				toggleMetrics = Create(asset, "Debug/ToggleMetrics");
			}
		}
	}
}
