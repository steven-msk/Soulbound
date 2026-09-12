namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Level;
	using System;

	public record NoiseSize(int minY, int height, int horizontalSize, int verticalSize) {
		public static readonly NoiseSize DEFAULT = Create(Level.DEFAULT_MIN_Y, Level.DEFAULT_WORLD_HEIGHT, 1, 1);

		public static NoiseSize Create(int minY, int height, int horizontalSize, int verticalSize) {
			return ValidateY(new NoiseSize(minY, height, horizontalSize, verticalSize)).GetOrThrow(m => new InvalidOperationException(m));
		}

		private static DataResult<NoiseSize> ValidateY(NoiseSize size) {
			const int MAX_TOP_Y = Level.DEFAULT_WORLD_HEIGHT + 1;
			return size.minY + size.height > MAX_TOP_Y
				? DataResult<NoiseSize>.Error("min_y + height cannot be greater than " + MAX_TOP_Y)
				: DataResult<NoiseSize>.Success(size);
		}

		public NoiseSize ClampToHeightLimit(IHeightLimitView heightLimit) {
			int newMinY = Math.Max(this.minY, heightLimit.GetBottomY());
			int newHeight = Math.Min(this.minY + this.height, heightLimit.GetTopY() + 1);
			return new NoiseSize(newMinY, newHeight, this.horizontalSize, this.verticalSize);
		}
	}
}
