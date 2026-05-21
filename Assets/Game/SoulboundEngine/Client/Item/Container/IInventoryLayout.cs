using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.Container {
	public interface IInventoryLayout {
		Vector2Int GetCoordinates(int index);
	}
}
