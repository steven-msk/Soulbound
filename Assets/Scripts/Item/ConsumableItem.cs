using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ConsumableItem")]
public class ConsumableItem : Item, IConsumable {
	[CanBeNull] [SerializeField] private ConsumableEffect consumeAction;
	public ConsumableEffect ConsumeAction => consumeAction;

	[SerializeField] private int consumeAmount;
	public int ConsumeAmount => consumeAmount;
}
