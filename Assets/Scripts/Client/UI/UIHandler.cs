using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public sealed class UIHandler {
		private Canvas canvas;
		private ScreenManager screenManager;

		public UIHandler(Canvas initialCanvas) {
			this.canvas = initialCanvas;
			this.screenManager = new ScreenManager(canvas.transform);
		}

		public void SetScreen(Screen? screen) {
			screenManager.SetScreen(screen);
		}

		public void SetCanvas(Canvas canvas) {
			this.canvas = canvas;
			screenManager = new ScreenManager(canvas.transform);
		}

		public void FlushScreens() => screenManager.Flush();

		public ScreenManager GetScreenManager() => screenManager;
	}
}
