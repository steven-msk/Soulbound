using System;

namespace SoulboundEngine.Client.Render.Item {
	public interface IUIToolkitSlotDisplay : IDisposable {
		public virtual void SetAsMainSlot() {
		}

		public virtual void UnsetMainSlot() {
		}
	}
}
