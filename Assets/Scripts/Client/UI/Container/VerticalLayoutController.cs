using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.UI {
	public sealed class VerticalLayoutController : IUILayoutController {
		private VerticalLayoutGroup group;

		public void ApplyTo(GameObject obj) {
			group = obj.GetOrAddComponent<VerticalLayoutGroup>();
			group.childControlHeight = true;
			group.childControlWidth = true;
			group.childForceExpandWidth = true;
			group.childForceExpandHeight = false;
		}

		public void OnChildAdded(UIElementNode node) {
		}
	}
}
