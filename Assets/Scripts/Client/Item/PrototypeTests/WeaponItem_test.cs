using SoulboundBackend.Client.Combat;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class WeaponItem_test : Item, IAttackSourceProvider {
	public override string name => "weaponItem_test";

	public override ItemAspect aspect => ItemAspectRegistry.Get(this, () => ItemAspect.Simple("fruit_icon"));

	public override int maxStackSize => 1;

	protected override Func<Item, TooltipData> tooltipSupplier => null;

	protected override TooltipRenderer.NodeStyleProvider nodeStyleProvider => null;

	public bool GetAttackSource(ItemUseTrigger trigger, out AttackSource source) {
		if (trigger == ItemUseTrigger.LeftClick) {
			//GameObject.Instantiate(Resources.Load<GameObject>("weaponItem_test_hitbox")).GetComponent<Hitbox>()
			source = new(10, 1, default);
			return true;
		}
		source = default;
		return false;
	}
}
