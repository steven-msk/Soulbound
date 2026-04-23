using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI {
	public record UIElementNode(GameObject gameObject) {
		public Transform transform => gameObject.transform;
	}
}
