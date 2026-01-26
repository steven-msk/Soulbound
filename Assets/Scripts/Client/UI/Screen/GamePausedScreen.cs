using Cysharp.Threading.Tasks;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.AssetManagement;
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
	public sealed class GamePausedScreen : Screen {
		private readonly Canvas canvas;
		private readonly UIManager uiManager;

		public GamePausedScreen() {
			this.canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			this.uiManager = canvas.GetComponent<UIManager>();
		}

		// prototypical; its only made to work with the ui backend rework
		[PROTOTYPICAL]
		public override ScreenObject BuildObject(Transform rootParent) {
			ScreenObject screen =  base.BuildObject(rootParent);
			ChildMap childMap = screen.GetChildMap();

			var title = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("GameMenuTitle")), rootParent);
			title.transform.SetParent(screen.transform);
			childMap.AddChild(title);


			var container = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("ButtonContainer")), rootParent);
			container.transform.SetParent(screen.transform);
			childMap.AddChild(container);


			var settingsButton = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("SettingsButton")), rootParent);
			settingsButton.transform.SetParent(container.transform);
			childMap.AddChild(settingsButton);

			var resumeButton = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("ResumeButton")), rootParent);
			resumeButton.transform.SetParent(container.transform);
			childMap.AddChild(resumeButton);

			var quitButton = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("QuitWorld")), rootParent);
			quitButton.transform.SetParent(container.transform);
			quitButton.GetComponent<Button>().onClick.AddListener(Soulbound.instance.QuitActiveWorld);
			childMap.AddChild(quitButton);

			return screen;
		}

		private async UniTask QuitWorld() {
			//LevelManager levelManager = Soulbound.instance.GetActiveLevelManager();
			//WorldManager worldManager = Soulbound.instance.worldManager;

			//worldManager.SaveWorld(levelManager.world, levelManager);
			//levelManager.StopSession();
			//Soulbound.instance.GetUIHandler().FlushScreens();
			//Time.timeScale = 1f;

			//await SceneManager.LoadSceneAsync("DevScene").ToUniTask();

			//var uiHandler = Soulbound.instance.GetUIHandler();
			//uiHandler.SetCanvas(UnityEngine.Object.FindFirstObjectByType<Canvas>());
			//uiHandler.SetScreen(new TitleScreen());
		}
	}
}
