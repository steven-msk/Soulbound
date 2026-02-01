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
				IUIElementContainer container = GUI.Container()
				.Layout(GUI.Layout.Vertical()
					.Align(UIAlignment.Center)
					.ControlChildSize(true)
				).Frame(new StretchFrame())
				.Build(screenObject);

			GUI.Button.New(GetAsset("ResumeButton")).Text("Resume").Build(container);
			GUI.Button.New(GetAsset("SettingsButton")).Text("Settings").Build(container);

			GUI.Button.New(GetAsset("QuitWorld")).Text("Quit To Title Screen")
				.OnClick(Soulbound.instance.QuitActiveWorld)
				.Build(container);
		}

		private GameObject GetAsset(string key) {
			return AssetManager.Resolve<GameObject>(new AssetKey(key));
		}
	}
}
