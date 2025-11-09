using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public class ChildReference : MonoBehaviour {
		[SerializeField] string referenceOverride;
		public string accessor => string.IsNullOrEmpty(referenceOverride)
				? gameObject.name
				: referenceOverride;

		public void OnRegisterChildrenReferences() {
			UnityEngine.Debug.Log($"registering child ref: {accessor}");
			SendMessageUpwards(
				"RegisterChildReference", this,
				SendMessageOptions.DontRequireReceiver
			);
		}
	}
}
