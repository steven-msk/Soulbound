using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IConsumable : IItemCapability {
	public int ConsumeAmount { get; }
	public ConsumableEffect ConsumeAction { get; }

	public virtual void Consume(ItemStack itemStack, PlayerController player) {
		ConsumeAction?.OnConsume(player);
		itemStack.Quantity -= ConsumeAmount;
	}
}
