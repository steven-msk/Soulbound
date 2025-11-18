using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	[PROTOTYPICAL]
	public class Screen : ChildReferenceContainer, IDisposable {
		public virtual void OnShow() {
			gameObject.SetActive(true);
		}

		public virtual void OnHide() {
			gameObject.SetActive(false);
		}

		public virtual void Dispose() {
			Destroy(gameObject);
		}
	}
}
