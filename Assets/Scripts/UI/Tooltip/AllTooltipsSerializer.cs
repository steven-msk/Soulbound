using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AllTooltipsSerializer : AbstractTooltipSerializer {
	public override ITooltipSerializer GetSerializer(Item item) => new AllTooltipsData(item);
}

public class AllTooltipsData : ITooltipSerializer {

	private readonly Item item;

	internal AllTooltipsData(Item item) {
		this.item = item;
	}

	public AbstractTooltip Generate() {
		return CompoundTooltip.Of(Tooltip.Title(item.Name), Tooltip.Stats(new Dictionary<IStatTypeImpl, object>() {
			[StatType<int>.MaxHealth] = -200,
			[StatType<int>.MaxMana] = -100,
			[StatType<int>.Defense] = -50,
			[StatType<int>.SoulSlots] = 5,
			[StatType<float>.MovementSpeed] = -1.2f,
			[StatType<int>.JumpHeight] = 2,
			[StatType<int>.MaxJumps] = 2,
			[StatType<float>.DashVelocity] = 2f,
			[StatType<float>.DashCooldown] = 0.1f,
			[StatType<float>.HealthRegen] = 1.7f,
			[StatType<float>.ManaRegen] = -2.5f,
			[StatType<int>.PhysicalDamage] = 250,
			[StatType<int>.RitualDamage] = 125,
			[StatType<float>.AttackSpeed] = -1.5f,
			[StatType<float>.CritChance] = 0.9f,
			[StatType<float>.CritMultiplier] = 2.6f,
			[StatType<float>.Luck] = 0.3f,
			[StatType<float>.LootBonus] = -0.6f
		}, applyAsBonus: true), Tooltip.Info("This item is used to display all existing tooltips"),Tooltip.Lore(item.LoreText));
	}
}

public class CustomTooltip : Tooltip {
	public CustomTooltip(TooltipData data) : base(data) {
	}

	public override void Update(ItemStack itemStack) {
		base.Update(itemStack);

	}
}
