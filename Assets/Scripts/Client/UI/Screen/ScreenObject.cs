using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public class ScreenObject : MonoBehaviour, IDisposable, IScreenObject {
		private Screen screenInstance;
		private ChildMap childMap;
		private Transform rootTransform;
		
		public void Init(Screen screenInstance, IScreenNavigator navigator, Transform rootTransform) {
			this.rootTransform = rootTransform;
			this.screenInstance = screenInstance;

			screenInstance.Init(navigator);
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

		public ChildReference AddChild(GameObject child) {
			child.transform.SetParent(transform, false);
			return childMap.AddChild(child);
		}

		// prototypical way to instantiate on this object
		// for some reason this is the only setup which sets the parent properly (sometimes)
		public ChildReference InstantiateChild(GameObject prefab) {
			var obj = GameObject.Instantiate(prefab, rootTransform);
			obj.transform.SetParent(transform, true);
			return AddChild(obj);
		}

		public void RemoveChild(string accessor) {
			childMap.RemoveChild(accessor);
		}
	}
}
