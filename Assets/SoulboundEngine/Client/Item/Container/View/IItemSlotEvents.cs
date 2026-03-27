using System;
using UnityEngine.EventSystems;

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public interface IItemSlotEvents {
		event Action<int, PointerEventData> pointerDown;
		event Action<int, PointerEventData> pointerUp;
		event Action<int, PointerEventData> pointerEnter;
		event Action<int, PointerEventData> pointerExit;
	}
}
