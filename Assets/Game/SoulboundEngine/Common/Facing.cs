namespace SoulboundEngine.Common.Math {
	using Math = System.Math;

	public readonly struct Facing {
		public static readonly Facing LEFT = new(-1.0d, 0.0d);
		public static readonly Facing RIGHT = new(1.0d, 0.0d);
		public static readonly Facing UP = new(0.0d, 1.0d);
		public static readonly Facing DOWN = new(0.0d, -1.0d);
		public readonly Vec2d direction;

		public Facing(double x) {
			this.direction = new Vec2d(x, 0f).Normalize();
		}

		public Facing(Vec2d vec) {
			this.direction = vec.Normalize();
		}

		public Facing(double x, double y) {
			this.direction = new Vec2d(x, y).Normalize();
		}

		public static Facing FromAngle(double angleDeg) {
			double rad = angleDeg * Maths.DEG_2_RAD;
			return new Facing(new Vec2d(Math.Cos(rad), Math.Sin(rad)));
		}

		public static Facing FromSignX(double x) {
			return new Facing(Math.Sign(x), 0.0d);
		}

		public static Facing FromSignY(double y) {
			return new Facing(0.0d, Math.Sign(y));
		}

		public static Facing FromSign(double x, double y) {
			return new Facing(Math.Sign(x), Math.Sign(y));
		}

		public double X => this.direction.x;
		public double Y => this.direction.y;
		public double AngleDeg => Math.Atan2(this.direction.y, this.direction.x) * Maths.RAD_2_DEG;
		public double AngleRad => Math.Atan2(this.direction.y, this.direction.x);
		public bool IsHorizontal => Math.Abs(this.direction.y) < 1.0E-5d;
	}
}
