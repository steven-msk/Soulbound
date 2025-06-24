using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ConsumableItem")]
public class ConsumableItem : Item, IConsumable {
	[CanBeNull] [SerializeField] private ConsumableEffect consumeAction;

	public ConsumableEffect ConsumeAction => consumeAction;

	[SerializeField] private int consumeAmount;
	public int ConsumeAmount => consumeAmount;

	protected override CompoundTooltip GetDefaultTooltip() => base.GetDefaultTooltip().Concat(Tooltip.Tag(ItemTag.Consumable));
}
