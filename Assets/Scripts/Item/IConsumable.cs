using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IConsumable : IItemCapability {
    public delegate void ConsumeAction(IConsumable consumable, ItemStack itemStack);

	public ConsumeAction consumeAction { get; }
    public int consumeAmount { get; }

	public virtual void Consume(ItemStack itemStack) {
		ConsumableUtils.DefaultConsume(this, itemStack);
	}
}

public static class ConsumableUtils {
	public static void DefaultConsume(IConsumable consumable, ItemStack itemStack) {
		consumable.consumeAction.Invoke(consumable, itemStack);
		itemStack.Quantity -= consumable.consumeAmount;
	}
}
