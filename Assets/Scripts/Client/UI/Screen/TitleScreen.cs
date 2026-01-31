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
			float leadingY = 0f;

			// prototypical
			foreach (var world in Soulbound.instance.ListWorldSaves()) {
				var button = CreateButton(world, ref leadingY, screenObject);
				button.onClick.AddListener(() => Soulbound.instance.EnterWorld(world));
			}

			if (leadingY < 0f) {
				leadingY -= 40f;
			}

			var newWorldButton = CreateButton("new world", ref leadingY, screenObject);
			newWorldButton.onClick.AddListener(() => {
				string world = $"world_{Guid.NewGuid()}";
				Soulbound.instance.CreateNewWorld(world);
				Soulbound.instance.EnterWorld(world);
			});
		}

		private Button CreateButton(string text, ref float leadingY, IScreenObject screen) {
			var prefab = AssetManager.Resolve<GameObject>(new AssetKey("WorldEnter"));
			var obj = screen.InstantiateChild(prefab);
			obj.GetComponent<TextMeshProUGUI>().text = text;
			obj.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, leadingY);
			leadingY -= 30f;
			return obj.GetComponent<Button>();
		}

	}
}
