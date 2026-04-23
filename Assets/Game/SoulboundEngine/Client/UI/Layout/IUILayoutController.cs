using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Layouts {
	public interface IUILayoutController {
		void ApplyTo(GameObject obj);
		void OnChildAdded(UIElementNode node);
		void OnChildRemoved(UIElementNode node);
	}
}
