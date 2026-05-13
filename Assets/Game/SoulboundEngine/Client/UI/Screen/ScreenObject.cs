using SoulboundEngine.Client.UI.Containers;
using SoulboundEngine.Client.UI.Screens;
using SoulboundEngine.Client.UI.Tooltips;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Screen {
	[Obsolete]
	public class ScreenObject : MonoBehaviour, IScreenObject, IDisposable {
		private Screen screenInstance;
		private ChildMap childMap;
		private readonly List<UITooltipNode> tooltipNodes = new();
		
		public void Init(Screen screenInstance) {
			this.screenInstance = screenInstance;
			this.childMap = new ChildMap();
		}

		public void Show() {
			this.gameObject.SetActive(true);
			//screenInstance.OnShow(this);
		}

		public void Hide() {
			//screenInstance.OnHide(this);
			this.gameObject.SetActive(false);
			this.DestroyTooltips();
		}

		public void Dispose() {
			//screenInstance.OnDispose(this);
			this.DestroyTooltips();
			Destroy(this.gameObject);
		}

		void IUIElementContainer.AddElement(UIElementNode node) {
			node.transform.SetParent(this.transform, false);
			this.childMap.AddChild(node.gameObject);
			((IUIElementContainer)this).OnElementAdded(node);
		}

		void IUIElementContainer.RemoveElement(UIElementNode node) {
			node.transform.SetParent(this.GetComponentInParent<Transform>(), false);
			this.childMap.RemoveChild(node.gameObject.name);
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
			node.transform.SetParent(this.transform, false);
			this.tooltipNodes.Add(node);
			node.handle.onDestroyed += () => {
				this.tooltipNodes.Remove(node);
			};
		}

		ITooltipHandle ITooltipRenderer.RenderTooltip(ITooltip tooltip) {
			return tooltip.Build(this);
		}

		private void DestroyTooltips() {
			foreach (var tooltipNode in this.tooltipNodes) {
				if (tooltipNode.isAlive) {
					Destroy(tooltipNode.gameObject);
				}
			}
			this.tooltipNodes.Clear();
		}

		public Screen GetInstance() => this.screenInstance;
	}
}
