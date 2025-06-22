using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableEffect_test", menuName = "Items/Effects/ConsumableEffect_test")]
public class ConsumableEffect_test : ConsumableEffect {
	public override void OnConsume(IConsumable consumable, ItemStack itemStack) {
		Debug.Log($"consumed {consumable.ConsumeAmount}, remaining: {itemStack.Quantity}");
	}
}
