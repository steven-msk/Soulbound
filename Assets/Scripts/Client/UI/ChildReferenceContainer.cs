using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	[DisallowMultipleComponent]
	public class ChildReferenceContainer : MonoBehaviour {
		protected ChildReferenceMap childMap = new();

		public virtual void RegisterChildReference(ChildReference reference) {
			UnityEngine.Debug.Log("register child: " + reference.gameObject.name);
			childMap.RegisterChildReference(reference);
		}

		public void BroadcastRegisterMessage() {
			BroadcastMessage("OnRegisterChildrenReferences", SendMessageOptions.DontRequireReceiver);
		}

		public void BruteForceRegisterAllChildren() {
			foreach (var childReference in GetComponentsInChildren<ChildReference>(true)) {
				childReference.OnRegisterChildrenReferences();
			}
		}

		public GameObject GetChild(string accessor) {
			return childMap.GetChild(accessor);
		}

		public T GetChildComponent<T>(string accessor) where T : Component {
			return GetChild(accessor).GetComponent<T>();
		}
	}
}
