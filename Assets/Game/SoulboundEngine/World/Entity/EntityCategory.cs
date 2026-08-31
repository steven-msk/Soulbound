namespace SoulboundEngine.World.Entity {
	using SoulboundEngine.Common;

	public readonly struct EntityCategory {
		public static readonly EntityCategory PLAYER = new(Color.CYAN);
		public static readonly EntityCategory OTHER = new(Color.WHITE);
		public readonly Color debugBoxColor;

		public EntityCategory(Color debugBoxColor) {
			this.debugBoxColor = debugBoxColor;
		}
	}
}
