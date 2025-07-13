using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;

public abstract class EquipableItem : Item, IEquipable {
	[SerializeField] private EquipableDelegate delegates;

	public virtual void OnEquip(EquipmentSlot slot) => delegates.NullOrElse(delegates => delegates.OnEquip(slot), LogWarning);

	public virtual void OnUnequipped() => delegates.NullOrElse(delegates => delegates.OnUnequip(this), LogWarning);

	private void LogWarning() => Debug.LogWarning($"Missing EquipableDelegate for item {this.name}");
}
