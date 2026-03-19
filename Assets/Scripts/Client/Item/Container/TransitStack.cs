using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container.View;
using SoulboundBackend.Client.ItemSystem.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem.Container {
	public sealed class TransitStack {
		private readonly IItemContainerScreenScope screenScope;
		private UIItemDisplay? display;

		public TransitStack(IItemContainerScreenScope screenScope) {
			this.screenScope = screenScope;
		}

		public void SetStack(ItemStack itemStack) {
			if (itemStack == null) {
				UnityEngine.Debug.LogException(new ArgumentException("TransitStack cannot be set to null. Call Release() instead"));
				return;
			}
			if (display != null) screenScope.RemoveTransitStack();

			display = new UIItemDisplay(null, itemStack);
			TransitStackHandle handle = new(display);

			display.onDestroyed += OnDisplayDestroyed;
			screenScope.SetTransitStackHandle(handle);
		}

		public void Release() {
			if (display != null) OnDisplayDestroyed();
		}

		public bool HasStack() => display != null;
		public ItemStack? GetStack() => display?.GetStack();

		private void OnDisplayDestroyed() {
			display!.onDestroyed -= OnDisplayDestroyed;
			screenScope.RemoveTransitStack();
			display = null;
		}
	}
}
