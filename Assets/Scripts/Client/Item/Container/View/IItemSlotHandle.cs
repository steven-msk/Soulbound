using SoulboundBackend.Client.UI;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.ItemSystem.Container.View {
	public interface IItemSlotHandle : IUIElementHandle, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
		void Init(IItemSlot slot);
	}
}
