using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public sealed class TransitStack {
		private readonly IItemContainerScreenScope screenScope;
		private ItemDisplay? display;

		public TransitStack(IItemContainerScreenScope screenScope) {
			this.screenScope = screenScope;
		}

		public void SetStack(ItemStack itemStack) {
			if (itemStack == null) {
				UnityEngine.Debug.LogException(new ArgumentException("TransitStack cannot be set to null. Call Release() instead"));
				return;
			}
			ItemStack? previous = GetStack();
			if (display != null) screenScope.RemoveTransitStack();

			display = ItemDisplay.Create(itemStack, () => null);
			ITransitStackHandle handle = display.AddComponent<TransitStackHandle>();

			handle.Init(display);
			display.onDestroy += OnDisplayDestroyed;
			screenScope.SetTransitStack(new UITransitStackNode(display.gameObject, handle));
		}

		public void Release() {
			if (display != null) OnDisplayDestroyed(display);
		}

		public bool HasStack() => display != null;
		public ItemStack? GetStack() => display != null
			? display.stack
			: null;

		private void OnDisplayDestroyed(ItemDisplay display) {
			display.onDestroy -= OnDisplayDestroyed;
			screenScope.RemoveTransitStack();
			this.display = null;
		}
	}
}
