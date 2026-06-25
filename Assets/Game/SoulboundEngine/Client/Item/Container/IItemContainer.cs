using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public interface IItemContainer {
		IItemSlot GetSlot(int index);
		IEnumerable<int> GetAllSlots();
		int GetSize();
	}
}
