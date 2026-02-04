using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public class ScreenObject : MonoBehaviour, IScreenObject, IDisposable {
		private Screen screenInstance;
		private ChildMap childMap;
		private readonly List<UITooltipNode> tooltipNodes = new();
		
		public void Init(Screen screenInstance, IScreenNavigator navigator) {
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
			DestroyTooltips();
		}

		public void Dispose() {
			screenInstance.OnDispose(this);
			DestroyTooltips();
			Destroy(gameObject);
		}

		void IUIElementContainer.AddElement(UIElementNode node) {
			node.transform.SetParent(transform, false);
			childMap.AddChild(node.gameObject);
		}

		void ITooltipManager.AddTooltip(UITooltipNode node) {
			node.transform.SetParent(transform, false);
			tooltipNodes.Add(node);
		}

		private void DestroyTooltips() {
			foreach (var tooltipNode in tooltipNodes) {
				if (tooltipNode.isAlive) {
					Destroy(tooltipNode.gameObject);
				}
			}
			tooltipNodes.Clear();
		}

		public Screen GetInstance() => screenInstance;
	}
}
