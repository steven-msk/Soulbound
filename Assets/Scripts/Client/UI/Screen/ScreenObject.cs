using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public class ScreenObject : MonoBehaviour, IDisposable {
		private Screen screenInstance;
		private ChildMap childMap;
		
		public void Init(Screen screenInstance) {
			this.screenInstance = screenInstance;
			this.childMap = new ChildMap();
		}

		public void Show() {
			gameObject.SetActive(true);
			screenInstance.OnShow();
		}

		public void Hide() {
			screenInstance.OnHide();
			gameObject.SetActive(false);
		}

		public void Dispose() {
			screenInstance.OnDispose();
			GameObject.Destroy(this.gameObject);
		}

		public Screen GetInstance() => screenInstance;

		public ChildMap GetChildMap() => childMap;
	}
}
