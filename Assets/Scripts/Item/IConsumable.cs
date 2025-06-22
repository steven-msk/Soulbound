using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IConsumable : IItemCapability {
	public int ConsumeAmount { get; }
	public ConsumableEffect ConsumeAction { get; }

	public virtual void Consume(ItemStack itemStack) {
		ConsumableUtils.DefaultConsume(this, itemStack);
	}
}

public static class ConsumableUtils {
	public static void DefaultConsume(IConsumable consumable, ItemStack itemStack) {
		consumable.ConsumeAction?.OnConsume(consumable, itemStack);
		itemStack.Quantity -= consumable.ConsumeAmount;
	}
}
