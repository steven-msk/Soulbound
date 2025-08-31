using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

#nullable enable

public class ArmorItem : StatItemDefinition, IEquipable {
    public ArmorType armorType { get; }
    public override bool applyInstantStatsAutomatically => true;

    public ArmorItem(string name, ArmorType armorType, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Item, TooltipData?> tooltipSupplier, 
			List<AbstractSerializableStat> instantStats, List<IBufferedStatImpl> bufferedStats, string interpolationSource)
		: base(name, icon, worldPrefabSupplier, maxStackSize, tooltipSupplier, instantStats, bufferedStats, interpolationSource) {
		this.armorType = armorType;
    }

    public void OnEquip(EquipmentSlot slot) {
		PlayerStats playerStats = GameManager.instance.Player.Stats;
		playerStats.Apply(this.instantStats, this);
		((IStatProvider)this).EnableBuffers(playerStats);
	}

	public void OnUnequipped() {
		PlayerStats playerStats = GameManager.instance.Player.Stats;
		playerStats.Revoke(instantStats, this);
		((IStatProvider)this).DisableBuffers(playerStats);
	}
}