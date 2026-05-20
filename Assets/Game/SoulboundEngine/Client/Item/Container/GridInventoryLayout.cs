using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.Container {
	public class GridInventoryLayout : IInventoryLayout {
		private readonly int width;
		private readonly int height;

		public GridInventoryLayout(int width, int height) {
			this.width = width;
			this.height = height;
		}

		public Vector2Int GetCoordinates(int index) {
			int row = index / this.width;
			int col = index / this.height;
			return new Vector2Int(row, col);
		}
	}
}
