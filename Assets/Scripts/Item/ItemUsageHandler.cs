using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ItemUsageHandler {
	private readonly Dictionary<(Type, ItemUseTrigger), Action<ItemStack>> handlers = new();
	private readonly PlayerController player;

	public ItemUsageHandler(PlayerController player) {
		this.player = player;
	}

	public void Register<T>(ItemUseTrigger trigger, Action<T, ItemStack> action) where T : Item {
		handlers[(typeof(T), trigger)] = itemStack => {
			if (itemStack.Item is T item) {
				action.Invoke(item, itemStack);
			}
		};
	}

	public void HandleInput(ItemUseTrigger trigger) {
		if (player.MainHandItem == null) {
			return;
		}

		if (handlers.TryGetValue((player.MainHandItem.Item.GetType(), trigger), out var action)) {
			action.Invoke(player.MainHandItem);
		}
	}
}
