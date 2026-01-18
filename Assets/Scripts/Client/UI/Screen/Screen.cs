using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public abstract class Screen {

		public virtual ScreenObject BuildObject(Transform rootParent) {
			GameObject obj = new("Screen Object");
			obj.transform.parent = rootParent;

			ScreenObject screenObject = obj.AddComponent<ScreenObject>();
			screenObject.Init(this);

			return screenObject;
		}

		public virtual void OnShow() {
		}

		public virtual void OnHide() {
		}

		public virtual void OnDispose() {
		}
	}
}
