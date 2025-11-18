using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

namespace SoulboundBackend.Client.UI {
	public class MainMenuScreen : IScreenBuilder {
		private readonly UIManager uiManager;

		public MainMenuScreen() {
			this.uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
		}

		public Screen GetScreen() {
			Screen screen = uiManager.screenChildMap.GetChild("MainMenu").GetComponent<Screen>();

			var enterButton = screen.GetChildComponent<Button>("WorldEnter");
			enterButton.onClick.RemoveAllListeners();
			enterButton.onClick.AddListener(Soulbound.instance.Prototype_LoadDevWorld);

			return screen;
		}
	}
}
