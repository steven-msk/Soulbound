using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	[PROTOTYPICAL]
	public class Screen : MonoBehaviour, IDisposable {
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

		public virtual void Dispose() {
			Destroy(gameObject);
		}
	}
}
