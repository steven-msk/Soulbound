using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct SerializedItemSlot {
	public int index;
	public ItemStack itemStack;

	public SerializedItemSlot(int index, ItemStack itemStack) {
		this.index = index;
		this.itemStack = itemStack;
	}
}
