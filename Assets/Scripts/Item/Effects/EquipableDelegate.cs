using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class EquipableDelegate : ItemUseEffect {

	public abstract void OnEquip(EquipmentSlot slot);

	public abstract void OnUnequip(Item item);
}