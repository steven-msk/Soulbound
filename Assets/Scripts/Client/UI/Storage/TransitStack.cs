using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public sealed class TransitStack {
		public event Action<ItemStack?, ItemStack?>? onStackChanged;
		private readonly IUIElementContainer transitContainer;
		private ItemDisplay? display;
		private UIElementNode node = null!;

		public TransitStack(IUIElementContainer transitContainer) {
			this.transitContainer = transitContainer;
		}

		public void SetStack(ItemStack itemStack) {
			if (itemStack == null) throw new ArgumentException("TransitStack cannot be set to null. Call Release() instead");

			ItemStack? previous = GetStack();
			if (display != null) display.Destroy();

			display = ItemDisplay.Create(itemStack, () => null);
			display.onDestroy += OnDisplayDestroyed;

			node = new UIElementNode(display.gameObject);
			transitContainer.AddElement(node);

			onStackChanged?.Invoke(previous, itemStack);
		}

		public void Release() {
			ItemStack? previous = GetStack();

			if (display != null) display.Destroy();
			display = null;

			if (previous != null) onStackChanged?.Invoke(previous, null);
		}

		public bool HasStack() => display != null;
		public ItemStack? GetStack() => display != null
			? display.stack
			: null;

		private void OnDisplayDestroyed(ItemStack stack) {
			transitContainer.RemoveElement(node);
			node = null!;
		}
	}
}
