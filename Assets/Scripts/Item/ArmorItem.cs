using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

#nullable enable

public abstract class ArmorItem : StatItem {
    public abstract ArmorType armorType { get; }
    public override bool applyInstantStatsOnHoverOrSelect => false;

    public void OnEquip(IStatReceiver receiver) {
		(this as IStatProvider).StartActivators(receiver);
		Debug.Log("equipped");
		//PlayerStats playerStats = GameManager.instance.Player.Stats;
		//(this as IStatProvider).ApplyStats(playerStats);
	}

	public void OnUnequipped(IStatReceiver receiver) {
		(this as IStatProvider).DiscardActivators(receiver);
		Debug.Log("unequipped");
		//PlayerStats playerStats = GameManager.instance.Player.Stats;
		//(this as IStatProvider).RevokeStats(playerStats);
	}
}