using System;
using UnityEngine;

namespace SoulboundEngine.Client.Item.Container {
	[Obsolete]
	public interface IInventoryLayout {
		Vector2Int GetCoordinates(int index);
	}
}
