using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public interface IChildContainer {
		ChildReference AddChild(GameObject child);
		ChildReference InstantiateChild(GameObject prefab);

		void RemoveChild(string accessor);
		public virtual void RemoveChild(ChildReference child) {
			RemoveChild(child.accessor);
		}
		public virtual void RemoveChild(GameObject child) {
			RemoveChild(child.GetComponent<ChildReference>());
		}

	}
}
