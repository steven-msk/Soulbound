using System;

namespace SoulboundEngine.Client.Render.Item {
	public sealed class ItemRenderHandle {
		private readonly object key;

		public ItemRenderHandle(object key) {
			this.key = key;
		}

		public override int GetHashCode() => HashCode.Combine(this.key);
	}
}
