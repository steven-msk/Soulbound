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

		protected override void OnBuild(IScreenObject screenObject) {
			//ScreenObject screen = (ScreenObject)base.BuildObject(rootParent);
			//ChildMap childMap = screen.GetChildMap();

			//screenObject.InstantiateChild(AssetManager.Resolve<GameObject>(new AssetKey("GameMenuTitle")));

			//ChildReference buttonContainer = screenObject.InstantiateChild(AssetManager.Resolve<GameObject>(new AssetKey("ButtonContainer")));

			//ChildReference settingsButton = screenObject.InstantiateChild(AssetManager.Resolve<GameObject>(new AssetKey("SettingsButton")));
			//settingsButton.transform.SetParent(buttonContainer.transform, false);
			//ChildReference resumeButton = screenObject.InstantiateChild(AssetManager.Resolve<GameObject>(new AssetKey("ResumeButton")));
			//resumeButton.transform.SetParent(buttonContainer.transform, false);
			//ChildReference quitWorld = screenObject.InstantiateChild(AssetManager.Resolve<GameObject>(new AssetKey("QuitWorld")));
			//quitWorld.transform.SetParent(buttonContainer.transform, false);
			//quitWorld.GetComponent<Button>().onClick.AddListener(Soulbound.instance.QuitActiveWorld);
			//var title = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("GameMenuTitle")), rootParent);
			//title.transform.SetParent(screen.transform);
			//childMap.AddChild(title);


			GUI.Button.New(GetAsset("ResumeButton")).Build(screenObject);
			GUI.Button.New(GetAsset("SettingsButton")).Build(screenObject);

			GUI.Button.New(GetAsset("QuitWorld"))
				.OnClick(Soulbound.instance.QuitActiveWorld)
				.Build(screenObject);


			//var container = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("ButtonContainer")), rootParent);
			//container.transform.SetParent(screen.transform);
			//childMap.AddChild(container);


			//var settingsButton = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("SettingsButton")), rootParent);
			//settingsButton.transform.SetParent(container.transform);
			//childMap.AddChild(settingsButton);

			//var resumeButton = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("ResumeButton")), rootParent);
			//resumeButton.transform.SetParent(container.transform);
			//childMap.AddChild(resumeButton);

			//var quitButton = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("QuitWorld")), rootParent);
			//quitButton.transform.SetParent(container.transform);
			//quitButton.GetComponent<Button>().onClick.AddListener(Soulbound.instance.QuitActiveWorld);
			//childMap.AddChild(quitButton);
		}

		private GameObject GetAsset(string key) {
			return AssetManager.Resolve<GameObject>(new AssetKey(key));
		}
	}
}
