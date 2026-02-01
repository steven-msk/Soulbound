using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public sealed class HorizontalLayoutController : IUILayoutController {
		private HorizontalLayoutGroup group;

		public void ApplyTo(GameObject obj) {
			group = obj.GetOrAddComponent<HorizontalLayoutGroup>();
			group.childControlHeight = true;
			group.childControlWidth = true;
			group.childForceExpandWidth = false;
			group.childForceExpandHeight = true;
		}

		public void OnChildAdded(UIElementNode node) {
		}
	}
}
