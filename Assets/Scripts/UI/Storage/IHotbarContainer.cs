using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IHotbarContainer : IContainer<InventorySlot> {
	new public int Columns { get; }

	int IContainer<InventorySlot>.Rows => 1;

	public InventorySlot[] Slots { get; }

	[Obsolete] InventorySlot IContainer<InventorySlot>.this[int row, int column]  => Slots[row];

	public InventorySlot this[int index] { get; }
}
