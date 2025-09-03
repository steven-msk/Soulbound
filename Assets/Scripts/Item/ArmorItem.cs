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
    public override bool applyInstantStatsOnHoverOrSelect => false;

    public void OnEquip(EquipmentSlot slot) {
		Debug.Log("equipped");
		PlayerStats playerStats = GameManager.instance.Player.Stats;
		playerStats.ApplyProvider(this);
	}

	public void OnUnequipped() {
		Debug.Log("unequipped");
		PlayerStats playerStats = GameManager.instance.Player.Stats;
		playerStats.RevokeProvider(this);
	}
}