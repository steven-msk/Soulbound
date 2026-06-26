namespace SoulboundEngine.Client.UI.Screen.Slot {
	public enum SlotDragActionType {
		/// <summary>
		/// Fills each dragged slot with a full stack clone of the transit stack
		/// </summary>
		CLONE,

		/// <summary>
		/// Splits the transit stack count across all dragged slots
		/// </summary>
		SPLIT,

		/// <summary>
		/// Inserts one item into each dragged slot
		/// </summary>
		INSERT
	}
}
