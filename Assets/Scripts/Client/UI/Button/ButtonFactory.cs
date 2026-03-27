namespace SoulboundBackend.Client.UI.Buttons {
	public sealed class ButtonFactory {
		public ButtonBuilder FromTemplate(IUIElementTemplate<LabelButtonHandle> template) {
			return new ButtonBuilder(template);
		}

		public ButtonBuilder Label() => FromTemplate(new LabelButton());
	}
}
