using SoulboundEngine.Client.ItemSystem.Container;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public sealed class HotbarSlotHandle : UIToolkitItemSlotHandle {
		public HotbarSlotHandle(VisualElement visualElement, IItemSlot slot, ItemRenderManager itemRenderManager) 
			: base(visualElement, slot, itemRenderManager) {
		}


	}
}
