using UnityEngine;

[CreateAssetMenu(menuName = "Items/ConsumableItem")]
public class ConsumableItem : Item {
	[SerializeField] private ConsumableEffect consumeAction;
	[SerializeField] private int consumeAmount;

	public void Consume(ItemStack itemStack, PlayerController player) {
		consumeAction?.OnConsume(player);
		itemStack.Quantity -= consumeAmount;
	}
}
