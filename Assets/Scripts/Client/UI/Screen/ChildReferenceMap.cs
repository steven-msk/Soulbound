using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public sealed class ChildReferenceMap {
		private Dictionary<string, GameObject> childReferences = new();

		public void RegisterChildReference(ChildReference reference) {
			childReferences[reference.accessor] = reference.gameObject;
		}

		public bool TryGetChild(string accessor, out GameObject child) {
			return childReferences.TryGetValue(accessor, out child);
		}

		public GameObject GetChild(string accessor) {
			if (!TryGetChild(accessor, out var child)) {
				throw new ArgumentException($"No child found with accessor: {accessor}");
			}
			return child;
		}
	}
}
