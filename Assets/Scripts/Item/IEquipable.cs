using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEquipable : IItemCapability {
	public void OnEquip(EquipmentSlot slot);

	public void OnUnequipped(Item item);
}