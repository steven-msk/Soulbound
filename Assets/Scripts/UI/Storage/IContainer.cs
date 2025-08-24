using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IContainer<TSlot> where TSlot : IItemSlot {
	public TSlot this[int row, int column] { get; }
	
	public int Rows { get; }

	public int Columns { get; }

	public TSlot GetSlotByIndex(int index);

	void SetupGrid();
}
