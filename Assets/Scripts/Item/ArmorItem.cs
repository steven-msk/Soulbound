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
	public event Action<IStatReceiver>? onEquipped;
	public event Action<IStatReceiver>? onUnequipped;
    public abstract ArmorType armorType { get; }
    public override bool applyInstantStatsOnHoverOrSelect => false;

    public void OnEquip(IStatReceiver receiver) {
		onEquipped?.Invoke(receiver);
		//(this as IStatProvider).BeginActivatorContexts(receiver);
		Debug.Log("equipped");
	}

	public void OnUnequipped(IStatReceiver receiver) {
		onUnequipped?.Invoke(receiver);
		//(this as IStatProvider).EndActivatorContexts(receiver);
		Debug.Log("unequipped");
	}
}