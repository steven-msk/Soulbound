using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IConsumable : IItemCapability {
	public int ConsumeAmount { get; }
	public ConsumableEffect ConsumeAction { get; }

	public virtual void Consume(ItemStack itemStack, PlayerController player) {
		ConsumableUtils.DefaultConsume(this, itemStack, player);
	}
}

public static class ConsumableUtils {
	public static void DefaultConsume(IConsumable consumable, ItemStack itemStack, PlayerController player) {
		consumable.ConsumeAction?.OnConsume(player);
		itemStack.Quantity -= consumable.ConsumeAmount;
	}
}
