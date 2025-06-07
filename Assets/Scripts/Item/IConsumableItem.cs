using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IConsumableItem {
	abstract void Consume(ItemStack itemStack, PlayerController player);
}
