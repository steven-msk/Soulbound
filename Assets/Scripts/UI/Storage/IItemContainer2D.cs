using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Graphs;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public interface IItemContainer2D : IItemContainer {
	public IItemSlot this[int row, int column] { get; }

	public int Rows { get; }
	public int Columns { get; }
}
