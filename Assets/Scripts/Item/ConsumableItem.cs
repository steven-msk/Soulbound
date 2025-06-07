using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Items/ConsumableItem")]
public class ConsumableItem : Item, IConsumableItem {

	public int consumeAmount;

	public void Consume(ItemStack itemStack, PlayerController player) {
		itemStack.Quantity -= consumeAmount;
	}
}
