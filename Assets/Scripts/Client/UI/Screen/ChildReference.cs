using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	[DisallowMultipleComponent]
	public class ChildReference : MonoBehaviour {
		[SerializeField] string referenceOverride;
		[SerializeField] private bool disableOnAwake = false;
		[SerializeField] private bool registerOnAwake = false;

		public string accessor => string.IsNullOrEmpty(referenceOverride)
				? gameObject.name
				: referenceOverride;

		private void Awake() {
			if (disableOnAwake) {
				foreach (var childRef in GetComponentsInChildren<ChildReference>(true)) {
					childRef.OnRegisterChildrenReferences();
				}
				gameObject.SetActive(false);
			}
			if (registerOnAwake) {
				OnRegisterChildrenReferences();
			}
		}

		public void OnRegisterChildrenReferences() {
			SendMessageUpwards(
				"RegisterChildReference", this,
				SendMessageOptions.DontRequireReceiver
			);
		}
	}
}
