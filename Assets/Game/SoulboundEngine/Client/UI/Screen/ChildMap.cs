using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Screens {
	public sealed class ChildMap {
		private readonly Dictionary<string, ChildReference> childRefs = new();

		public ChildReference AddChild(GameObject obj) {
			ChildReference childRef = obj.GetOrAddComponent<ChildReference>();
			childRefs[childRef.accessor] = childRef;
			return childRef;
		}

		public bool TryGetChild(string accessor, out ChildReference child) {
			return childRefs.TryGetValue(accessor, out child);
		}

		public ChildReference GetChild(string accessor) {
			if (!TryGetChild(accessor, out var child)) {
				throw new ArgumentException($"No child found with accessor: {accessor}");
			}
			return child;
		}

		public void RemoveChild(string accessor) {
			childRefs.Remove(accessor);
		}
	}
}
