namespace SoulboundEngine.Client.World.Widget {
	public class WorldWidgetHandle {
		public WorldWidgetType type { get; }
		public WorldWidgetContext context { get; private set; }

		public WorldWidgetHandle(WorldWidgetType type, WorldWidgetContext context) {
			this.type = type;
			this.context = context;
		}

		public void UpdateContext(WorldWidgetContext context) {
			this.context = context;
		}
	}
}
