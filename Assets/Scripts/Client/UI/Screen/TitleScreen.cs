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
using UnityEngine.UI;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

namespace SoulboundBackend.Client.UI {
	public class TitleScreen : Screen {
		private readonly UIManager uiManager;

		public TitleScreen() {
			this.uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
		}

		public override ScreenObject BuildObject(Transform rootParent) {
			ScreenObject screen = base.BuildObject(rootParent);

			var prefab = AssetManager.Resolve<GameObject>(new AssetKey("WorldEnter"));
			float leadingY = 0f;

			// prototypical
			var saves = Soulbound.instance.worldManager.ListSaves().ToList();
			UnityEngine.Debug.Log(saves.Count);
			foreach (var world in saves) {
				var obj = GameObject.Instantiate(prefab, rootParent);
				obj.GetComponent<TextMeshProUGUI>().text = world;
				obj.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, leadingY);
				leadingY -= 30f;

				obj.transform.SetParent(screen.transform);
				var b = obj.GetComponent<Button>();

				b.onClick.AddListener(Soulbound.instance.Prototype_LoadDevWorld);
				screen.GetChildMap().AddChild(obj);
			}

			return screen;
		}
	}
}
