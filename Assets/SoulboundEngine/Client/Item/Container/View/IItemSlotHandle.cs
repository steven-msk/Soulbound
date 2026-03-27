using SoulboundEngine.Client.UI;
using UnityEngine.EventSystems;

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public interface IItemSlotHandle : IUIElementHandle, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
		void Init(IItemSlot slot);
	}
}
