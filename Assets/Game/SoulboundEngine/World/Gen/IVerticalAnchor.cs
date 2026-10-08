namespace SoulboundEngine.World.Gen {
	public interface IVerticalAnchor {
		int ResolveY(WorldGenContext context);

		public static IVerticalAnchor Top => BelowTop(0);

		public static IVerticalAnchor Bottom => AboveBottom(0);

		public static IVerticalAnchor AboveBottom(int offset) => new AboveBottomOffset(offset);

		public static IVerticalAnchor Absolute(int y) => new AbsoluteY(y);

		public static IVerticalAnchor BelowTop(int offset) => new BelowTopOffset(offset);

		public record AboveBottomOffset(int offset) : IVerticalAnchor {
			public int ResolveY(WorldGenContext context) {
				return context.minGenY + this.offset;
			}

			public override string ToString() {
				return $"{this.offset} above bottom";
			}
		}

		public record AbsoluteY(int y) : IVerticalAnchor {
			public int ResolveY(WorldGenContext context) => this.y;

			public override string ToString() {
				return $"{this.y} absolute";
			}
		}

		public record BelowTopOffset(int offset) : IVerticalAnchor {
			public int ResolveY(WorldGenContext context) {
				return context.minGenY - 1 + context.height - this.offset;
			}

			public override string ToString() {
				return $"{this.offset} below top";
			}
		}
	}
}
