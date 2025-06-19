using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/ConsumableEffect_test")]
public class ConsumableEffect_test : ConsumableEffect {
	public override void OnConsume(PlayerController player) {
		Debug.Log("consumed");
	}
}
