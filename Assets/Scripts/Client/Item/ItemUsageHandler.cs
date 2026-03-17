using SoulboundBackend.Client;
using SoulboundBackend.Client.World;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.ItemSystem {
	[Obsolete]
    public sealed class ItemUsageHandler {
		private readonly Dictionary<(Type itemCapability, ItemInteractionTrigger useTrigger), Action<ItemStack>> handlers = new();
		private readonly List<ItemInteractionTrigger> disabledTriggers = new();
		private readonly Player player;

		public ItemUsageHandler(Player player) {
			this.player = player;
		}

		public void RegisterCapability<T>(ItemInteractionTrigger trigger, Action<T, ItemStack> action) {
			handlers[(typeof(T), trigger)] = (itemStack => {
				if (itemStack.item is T item) {
					action.Invoke(item, itemStack);
				}
			});
		}

		public void Enable(params ItemInteractionTrigger[] triggers) => triggers.ToList().ForEach((trigger) => disabledTriggers.Remove(trigger));

		public void Disable(params ItemInteractionTrigger[] triggers) => disabledTriggers.AddRange(triggers);

		public void HandleInput(ItemInteractionTrigger trigger, ItemStack itemStack) {
			if (itemStack == null || IsDisabled(trigger)) {
				return;
			}
			foreach (var (itemCapability, useTrigger) in handlers.Keys) {
				if (!itemCapability.IsInstanceOfType(itemStack.item) || useTrigger != trigger) {
					continue;
				}
				if (handlers.TryGetValue((itemCapability, useTrigger), out var action)) {
					action?.Invoke(itemStack);
				}
				if (itemStack == null) {
					break;
				}
			}
		}

		public bool IsDisabled(ItemInteractionTrigger trigger) => disabledTriggers.Contains((trigger));
    }
}

