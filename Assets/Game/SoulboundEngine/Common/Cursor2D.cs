namespace SoulboundEngine.Common {
	public class Cursor2D {
		public const int INSIDE = 0;
		public const int EDGE = 1;
		public const int CORNER = 2;
		private readonly int originX;
		private readonly int originY;
		private readonly int width;
		private readonly int height;
		private readonly int end;
		private int index;
		private int x;
		private int y;

		public Cursor2D(int minX, int minY, int maxX, int maxY) {
			this.originX = minX;
			this.originY = minY;
			this.width = maxX - minX + 1;
			this.height = maxY - minY + 1;
			this.end = this.width * this.height;
		}

		public bool Advance() {
			if (this.index == this.end) return false;

			this.x = this.index % this.width;
			int slice = this.index / this.width;
			this.y = slice % this.height;
			this.index++;
			return true;
		}

		public int NextX() => this.originX + this.x;
		public int NextY() => this.originY + this.y;

		public int GetNextType() {
			int type = 0;

			if (this.x == 0 || this.x == this.width - 1) {
				type++;
			}

			if (this.y == 0 || this.y == this.height - 1) {
				type++;
			}

			return type;
		}
	}
}
