using SoulboundEngine.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Containers {
	public interface IUIElementContainer {
		void AddElement(UIElementNode node);
		void RemoveElement(UIElementNode node);
		void OnElementAdded(UIElementNode node);
		void OnElementRemoved(UIElementNode node);
	}
}
