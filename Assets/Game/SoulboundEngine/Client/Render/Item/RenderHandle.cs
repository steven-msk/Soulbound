using System;

namespace SoulboundEngine.Client.Render.Item {
	public sealed class RenderHandle {
		private readonly object key;

		public RenderHandle(object key) {
			this.key = key;
		}

		public override int GetHashCode() => HashCode.Combine(this.key);
	}
}
