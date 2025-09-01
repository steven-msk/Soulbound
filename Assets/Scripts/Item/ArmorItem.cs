using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

#nullable enable

public abstract class ArmorItem : StatItem, IEquipable {
    public abstract ArmorType armorType { get; }
    public override bool applyInstantStatsOnHoverOrSelect => true;

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