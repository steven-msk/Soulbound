using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI.Screens {
	[PROTOTYPICAL]
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

			Button settingsButton = screen.GetChild("SettingsButton").GetComponent<Button>();
			settingsButton.onClick.RemoveAllListeners();
			settingsButton.onClick.AddListener(() => uiManager.SetScreen<SettingsScreen>());

			Button resumeButton = screen.GetChild("ResumeButton").GetComponent<Button>();
			resumeButton.onClick.RemoveAllListeners();
			resumeButton.onClick.AddListener(Soulbound.instance.GetActiveLevelManager().TogglePause);

			Button quitButton = screen.GetChild("QuitWorld").GetComponent<Button>();
			quitButton.onClick.RemoveAllListeners();
			quitButton.onClick.AddListener(() => CoroutineRunner.GetInstance().StartCoroutine(QuitWorld()));

			return screen;
		}

		private IEnumerator QuitWorld() {
			LevelManager levelManager = Soulbound.instance.GetActiveLevelManager();
			WorldManager worldManager = Soulbound.instance.worldManager;

			worldManager.SaveWorld(levelManager.world, levelManager);
			levelManager.StopSession();
			var async = SceneManager.LoadSceneAsync("DevScene");
			yield return new WaitUntil(() => async.isDone);
			Time.timeScale = 1f;

			var uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
			uiManager.SetScreen(new TitleScreen());
		}
	}
}
