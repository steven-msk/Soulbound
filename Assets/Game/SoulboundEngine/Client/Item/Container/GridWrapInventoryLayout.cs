using System;
using UnityEngine;

namespace SoulboundEngine.Client.Item.Container {
	[Obsolete]
	public class GridWrapInventoryLayout : IInventoryLayout {
		private readonly int wrapWidth;

		public GridWrapInventoryLayout(int wrapWidth) {
			this.wrapWidth = wrapWidth;
		}

		public Vector2Int GetCoordinates(int index) {
			int row = index % this.wrapWidth;
			int col = index / this.wrapWidth;
			return new Vector2Int(row, col);
		}
	}
}
