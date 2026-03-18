using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI {
	public interface IItemSlotEvents {
		event Action<int, PointerEventData> pointerDown;
		event Action<int, PointerEventData> pointerUp;
		event Action<int, PointerEventData> pointerEnter;
		event Action<int, PointerEventData> pointerExit;
	}
}
