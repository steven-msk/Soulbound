using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public sealed class ChildReferenceMap {
		private Dictionary<string, GameObject> childReferences = new();

		public void RegisterChildReference(ChildReference reference) {
			childReferences[reference.accessor] = reference.gameObject;
		}

		public GameObject GetChild(string accessor) {
			return childReferences[accessor];
		}
	}
}
