namespace SoulboundEngine.Client.UI.Screen.Slot {
	public enum SlotActionType {
		/// <summary>
		/// Clones the item into the slot or transit stack.
		/// </summary>
		CLONE,

		/// <summary>
		/// Fills the transit stack with items from the inventory screen handler.
		/// This is usually triggered by the player double clicking.
		/// </summary>
		COLLECT_ALL,

		/// <summary>
		/// Performs a normal slot click. This can pickup or place items in the slot, 
		/// possibly merging the cursor stack into the slot, 
		/// or swapping the slot stack with the transit stack if they can't merge.
		/// </summary>
		PICKUP,

		/// <summary>
		/// Quick moves the stack to an available slot in another inventory
		/// </summary>
		QUICK_MOVE
	}
}
