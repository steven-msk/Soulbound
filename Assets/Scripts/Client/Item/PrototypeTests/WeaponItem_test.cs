using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Attack;
using SoulboundBackend.Client.Stats;
using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem_test : WeaponItem {
	public override GameObject attackPrefab { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
	public override WeaponAttackBehavior attackBehavior { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

	public override string name => throw new NotImplementedException();

	public override ItemAspect aspect => throw new NotImplementedException();

	public override int maxStackSize => throw new NotImplementedException();

	public override IEnumerable<StatMapping> statMappings => throw new NotImplementedException();

	protected override Func<Item, TooltipData> tooltipSupplier => throw new NotImplementedException();

	protected override TooltipRenderer.NodeStyleProvider nodeStyleProvider => throw new NotImplementedException();
}
