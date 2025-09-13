using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public interface IConsumable : IItemCapability {
    public delegate void ConsumeAction(IConsumable consumable, ItemStack itemStack);

	public ConsumeAction? consumeAction { get; }
    public int consumeAmount { get; }

	public virtual void Consume(ItemStack itemStack) {
		Consumables.DefaultConsume(this, itemStack);
	}
}

public static class Consumables {
	public static void DefaultConsume(IConsumable consumable, ItemStack itemStack) {
		consumable.consumeAction?.Invoke(consumable, itemStack);
		itemStack.Decrement(consumable.consumeAmount);
	}
}
