using SoulboundEngine.Common.Math;
using SoulboundEngine.World.Physics;

namespace SoulboundEngine.World.Entity {
	public readonly struct EntityDimensions {
		public readonly double width;
		public readonly double height;
		public readonly bool isFixed;

		public EntityDimensions(double width, double height, bool isFixed) {
			this.width = width;
			this.height = height;
			this.isFixed = isFixed;
		}

		public static EntityDimensions Scalable(double width, double height) {
			return new EntityDimensions(width, height, false);
		}

		public static EntityDimensions Fixed(double width, double height) {
			return new EntityDimensions(width, height, true);
		}

		public AABB MakeBoundingBox(Vec2d pos) {
			return this.MakeBoundingBox(pos.x, pos.y);
		}

		public AABB MakeBoundingBox(double x, double y) {
			double halfWidth = this.width * 0.5d;
			double height = this.height;
			return new AABB(x - halfWidth, y, x + halfWidth, y + height);
		}

		public EntityDimensions Scale(double scale) {
			return this.Scale(scale, scale);
		}

		public EntityDimensions Scale(double scaleX, double scaleY) {
			if (this.isFixed || (scaleX == 1.0d && scaleY == 1.0d)) return this;
			return new EntityDimensions(this.width * scaleX, this.height * scaleY, false);
		}
	}
}
