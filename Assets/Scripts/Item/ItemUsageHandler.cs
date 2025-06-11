using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class ItemUsageHandler {
	private readonly Dictionary<(Type itemCapability, ItemUseTrigger useTrigger), Action<ItemStack>> handlers = new();
	private readonly PlayerController player;

	public ItemUsageHandler(PlayerController player) {
		this.player = player;
	}

	public void Register<T>(ItemUseTrigger trigger, Action<T, ItemStack> action) where T : IItemCapability {
		handlers[(typeof(T), trigger)] = itemStack => {
			if (itemStack.Item is T item) {
				action.Invoke(item, itemStack);
			}
		};
	}

	public void HandleInput(ItemUseTrigger trigger) {
		if (player.MainHandItem == null || !EventPriorityManager.IsAllowed("ItemUse")) {
			return;
		}
		foreach (var (itemCapability, useTrigger) in handlers.Keys) {
			if (!itemCapability.IsInstanceOfType(player.MainHandItem.Item) || useTrigger != trigger) {
				continue;
			}
			if (handlers.TryGetValue((itemCapability, useTrigger), out var action)) {
				action?.Invoke(player.MainHandItem);
			}
			if (player.MainHandItem == null) {
				break;
			}
		}
	}
}

