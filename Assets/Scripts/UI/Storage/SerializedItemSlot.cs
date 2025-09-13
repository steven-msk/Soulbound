public struct SerializedItemSlot {
	public int index;
	public ItemStack itemStack;

	public SerializedItemSlot(int index, ItemStack itemStack) {
		this.index = index;
		this.itemStack = itemStack;
	}
}
