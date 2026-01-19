using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Core;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			// this makes no sense but its the only way its parenting it properly
			var prefab = ResourceManager.Get<GameObject, ResourceGroups.UI>(new AssetKey("WorldEnter"));
			var obj = GameObject.Instantiate(prefab, rootParent);
			obj.transform.SetParent(screen.transform);

			var b = obj.GetComponent<Button>();
			b.onClick.AddListener(Soulbound.instance.Prototype_LoadDevWorld);

			screen.GetChildMap().AddChild(obj);
			return screen;
		}
	}
}
