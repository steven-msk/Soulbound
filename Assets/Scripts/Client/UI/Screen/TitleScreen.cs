using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Core;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UI;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

namespace SoulboundBackend.Client.UI {
	public class TitleScreen : Screen {
		protected override void OnBuild(IScreenObject screenObject) {
			var prefab = AssetManager.Resolve<GameObject>(new AssetKey("WorldEnter"));

			foreach (var world in Soulbound.instance.ListWorldSaves()) {
				GUI.Button.New(prefab)
					.Text(world)
					.OnClick(() => Soulbound.instance.EnterWorld(world))
					.Build(screenObject);
			}

			GUI.Button.New(prefab)
				.Text("new world")
				.OnClick(() => {
					string world = $"world_{Guid.NewGuid()}";
					Soulbound.instance.CreateNewWorld(world);
					Soulbound.instance.EnterWorld(world);
				})
				.Build(screenObject);
		}
	}
}
