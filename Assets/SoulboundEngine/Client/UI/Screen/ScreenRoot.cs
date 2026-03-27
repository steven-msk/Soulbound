using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Screens {
	public sealed class ScreenRoot : IScreenRoot {
		private readonly Transform rootTransform;

		public ScreenRoot(Transform rootTransform) {
			this.rootTransform = rootTransform;
		}

		public void AttachScreenObject(GameObject screenObject) {
			screenObject.transform.SetParent(rootTransform, false);
		}
	}
}
