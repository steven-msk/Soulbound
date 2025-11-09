using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public class Screen : MonoBehaviour {
		public ChildReferenceMap childMap { get; } = new();

		public virtual void OnShow() {
			gameObject.SetActive(true);
		}

		public virtual void OnHide() {
			gameObject.SetActive(false);
		}

		public void RegisterChildReference(ChildReference reference) {
			UnityEngine.Debug.Log($"caught child ref: {reference.accessor} @ {name}");
			childMap.RegisterChildReference(reference);
		}
	}
}
