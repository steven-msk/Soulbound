using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace SoulboundBackend.Client.UI {
	public interface IItemSlotHandle : IUIElementHandle, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
		void Init(IItemSlot slot);
	}
}
