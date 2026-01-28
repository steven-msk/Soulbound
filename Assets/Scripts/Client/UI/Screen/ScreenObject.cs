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
			childMap = new ChildMap();
		}

		public void Show() {
			gameObject.SetActive(true);
			screenInstance.OnShow(this);
		}

		public void Hide() {
			screenInstance.OnHide(this);
			gameObject.SetActive(false);
		}

		public void Dispose() {
			screenInstance.OnDispose(this);
			Destroy(gameObject);
		}

		public Screen GetInstance() => screenInstance;

		public ChildMap GetChildMap() => childMap;
	}
}
