using SoulboundBackend.Client.UI.Containers;
using SoulboundBackend.Client.UI.Tooltips;
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
			((IUIElementContainer)this).OnElementAdded(node);
		}

		void IUIElementContainer.RemoveElement(UIElementNode node) {
			node.transform.SetParent(GetComponentInParent<Transform>(), false);
			childMap.RemoveChild(node.gameObject.name);
			((IUIElementContainer)this).OnElementRemoved(node);
		}

		void IUIElementContainer.OnElementAdded(UIElementNode node) {
			foreach (var tooltipTrigger in node.gameObject.GetComponentsInChildren<ITooltipTrigger>(true)) {
				tooltipTrigger.Init(this);
			}
		}

		void IUIElementContainer.OnElementRemoved(UIElementNode node) {
		}

		void ITooltipManager.AddTooltip(UITooltipNode node) {
			node.transform.SetParent(transform, false);
			tooltipNodes.Add(node);
			node.handle.onDestroyed += () => {
				tooltipNodes.Remove(node);
			};
		}

		ITooltipHandle ITooltipRenderer.RenderTooltip(ITooltip tooltip) {
			return tooltip.Build(this);
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
