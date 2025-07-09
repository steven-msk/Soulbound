using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IContainer {
	public IItemSlot this[int row, int column] { get; }
	
	public int Rows { get; }

	public int Columns { get; }

#nullable enable

	public void SetupGrid(Action? callback);

#nullable disable
}
