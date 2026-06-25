using UnityEngine;

namespace SoulboundEngine.Client.Item.Container {
	public interface IInventoryLayout {
		Vector2Int GetCoordinates(int index);
	}
}
