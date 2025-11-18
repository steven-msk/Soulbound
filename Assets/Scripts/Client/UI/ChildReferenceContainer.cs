using SoulboundBackend.Client.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public class ChildReferenceContainer : MonoBehaviour {
		private ChildReferenceMap childMap = new();

		// Received message
		public void RegisterChildReference(ChildReference reference) {
			childMap.RegisterChildReference(reference);
		}

		public GameObject GetChild(string accessor) {
			return childMap.GetChild(accessor);
		}

		public T GetChildComponent<T>(string accessor) where T : Component {
			return GetChild(accessor).GetComponent<T>();
		}
	}
}
