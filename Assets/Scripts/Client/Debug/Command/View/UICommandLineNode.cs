using SoulboundBackend.Client.UI;

namespace SoulboundBackend.Client.Debug.Commands.View {
	public record UICommandLineNode : UIOverlayNode {
		public readonly ICommandLineHandler handler;

		protected UICommandLineNode(UIOverlayNode original, ICommandLineHandler handler)
			: base(original) {
			this.handler = handler;
		}
	}
}
