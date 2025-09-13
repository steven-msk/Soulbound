using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

public interface IConsumable : IItemCapability {
    public delegate void ConsumeAction(IConsumable consumable, ItemStack itemStack);

	public ConsumeAction? consumeAction { get; }
    public int consumeAmount { get; }
	public IEnumerable<IConsumptionRestriction>? restrictions { get; }

	public virtual bool CanConsume(ItemStack itemStack) {
		return restrictions == null || restrictions.All(r => r.CanConsume(this, itemStack));
	}

	public virtual void Consume(ItemStack itemStack) {
		Consumables.DefaultConsume(this, itemStack);
	}
}

public static class Consumables {
	public static void DefaultConsume(IConsumable consumable, ItemStack itemStack) {
		Debug.Log("attempting to consume");
		if (consumable.CanConsume(itemStack)) {
			consumable.consumeAction?.Invoke(consumable, itemStack);
			itemStack.Decrement(consumable.consumeAmount);
			consumable.restrictions.ToList().ForEach(r => r.NotifyConsumed(itemStack));
			Debug.Log("successfully consumed");
		}
	}
}
