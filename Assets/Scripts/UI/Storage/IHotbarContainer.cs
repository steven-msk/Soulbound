using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IHotbarContainer : IContainer {
	new public int Columns { get; }

	int IContainer.Rows => 1;

	public InventorySlot[] Slots { get; }

	[Obsolete] IItemSlot IContainer.this[int row, int column]  => Slots[row];

	public InventorySlot this[int index] { get; }
}
