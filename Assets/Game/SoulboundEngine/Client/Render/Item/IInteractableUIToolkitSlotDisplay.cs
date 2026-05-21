using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public interface IInteractableUIToolkitSlotDisplay : IUIToolkitSlotDisplay {
		event Action<PointerDownEvent> onPointerDown;
		event Action<PointerEnterEvent> onPointerEnter;
		event Action<PointerLeaveEvent> onPointerLeave;
		event Action<PointerUpEvent> onPointerUp;
	}
}