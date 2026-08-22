namespace SoulboundEngine.World.Widget {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Level;

	public class TextWidgetHandler : WorldWidgetHandler<TextWidgetHandler.Context> {
		private TextWidgetHandler(WorldWidgetType<Context> widgetType, Context context)
			: base(widgetType, context) {
		}

		public static TextWidgetHandler Create(WorldWidgetType<Context> widgetType, Context context) {
			return new TextWidgetHandler(widgetType, context);
		}

		protected override void OnInit(Context context) {
		}

		protected override void OnUpdate(Context oldContext, Context context) {
		}

		public void SetText(string text) {
			this.Update(this.GetCurrentContext().SetText(text));
		}

		public record Context(Level level, BlockPos blockPos, string text) : WorldWidgetContext(level, blockPos) {
			public Context SetText(string text) {
				return new Context(this.level, this.blockPos, text);
			}
		}
	}
}
