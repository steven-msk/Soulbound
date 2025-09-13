public interface IItemContainer2D : IItemContainer {
	public IItemSlot this[int row, int column] { get; }

	public int Rows { get; }
	public int Columns { get; }
}
