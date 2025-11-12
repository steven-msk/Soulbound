using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI.Screens {
	public sealed class GamePausedScreen : IScreenBuilder {
		private readonly Canvas canvas;
		private readonly UIManager uiManager;

		public GamePausedScreen() {
			this.canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			this.uiManager = canvas.GetComponent<UIManager>();
		}

		public Screen GetScreen() {
			GameObject screenObject = GameObject.Instantiate(ResourceManager.GetRuntimePrefab("GamePausedMenu"), canvas.transform);
			Screen screen = screenObject.GetComponent<Screen>();
			screen.BroadcastMessage("OnRegisterChildrenReferences", SendMessageOptions.DontRequireReceiver);

			Button settingsButton = screen.childMap.GetChild("SettingsButton").GetComponent<Button>();
			settingsButton.onClick.RemoveAllListeners();
			settingsButton.onClick.AddListener(() => uiManager.SetScreen<SettingsScreen>());

			Button resumeButton = screen.childMap.GetChild("ResumeButton").GetComponent<Button>();
			resumeButton.onClick.RemoveAllListeners();
			resumeButton.onClick.AddListener(() => Soulbound.instance.GetActiveLevelManager().TogglePause());

			return screen;
		}
	}
}
