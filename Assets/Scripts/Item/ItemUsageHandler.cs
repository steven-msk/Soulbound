using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public sealed class ItemUsageHandler {
	private readonly Dictionary<(Type itemCapability, ItemUseTrigger useTrigger), Action<ItemStack>> handlers = new();
	private readonly List<ItemUseTrigger> disabledTriggers = new();
	private readonly PlayerController player;

	public ItemUsageHandler(PlayerController player) {
		this.player = player;
	}

	public void Register<T>(ItemUseTrigger trigger, Action<T, ItemStack> action) where T : IItemCapability {
		handlers[(typeof(T), trigger)] = (itemStack => {
			if (itemStack.Item is T item) {
				action.Invoke(item, itemStack);
			}
		});
	}

	public void Enable(params ItemUseTrigger[] triggers) => triggers.ToList().ForEach((trigger) => disabledTriggers.Remove(trigger));

	public void Disable(params ItemUseTrigger[] triggers) => disabledTriggers.AddRange(triggers);

	public void HandleInput(ItemUseTrigger trigger, ItemStack itemStack) {
		if (itemStack == null || IsDisabled(trigger)) {
			return;
		}
		foreach (var (itemCapability, useTrigger) in handlers.Keys) {
			if (!itemCapability.IsInstanceOfType(itemStack.Item) || useTrigger != trigger) {
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

	public bool IsDisabled(ItemUseTrigger trigger) => disabledTriggers.Contains((trigger));
}

