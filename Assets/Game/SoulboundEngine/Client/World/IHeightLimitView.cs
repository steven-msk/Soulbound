using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Chunk;

namespace SoulboundEngine.Client.World {
	public interface IHeightLimitView {
		int GetBottomY();
		int GetHeight();

		public static IHeightLimitView Create(int bottomY, int height) {
			return new SimpleIml(bottomY, height);
		}

		private sealed class SimpleIml : IHeightLimitView {
			private readonly int bottomY;
			private readonly int height;

			public SimpleIml(int bottomY, int height) {
				this.bottomY = bottomY;
				this.height = height;
			}

			int IHeightLimitView.GetBottomY() => this.bottomY;
			int IHeightLimitView.GetHeight() => this.height;
		}
	}

	public static class HeightLimitDefaults {
		public static int GetTopY(this IHeightLimitView heightLimitView) {
			return heightLimitView.GetBottomY() + heightLimitView.GetHeight() - 1;
		}

		public static bool IsInHeightLimit(this IHeightLimitView heightLimitView, int y) {
			return y <= heightLimitView.GetTopY() && y >= heightLimitView.GetBottomY();
		}

		public static bool IsOutOfHeightLimit(this IHeightLimitView heightLimitView, int y) {
			return y < heightLimitView.GetBottomY() || y > heightLimitView.GetTopY();
		}

		public static bool IsOutOfHeightLimit(this IHeightLimitView heightLimitView, BlockPos blockPos) {
			return heightLimitView.IsOutOfHeightLimit(blockPos.y);
		}

		public static int GetBottomSectionY(this IHeightLimitView heightLimitView) {
			return SectionPos.BlockToSectionY(heightLimitView.GetBottomY());
		}

		public static int GetTopSectionY(this IHeightLimitView heightLimitView) {
			return SectionPos.BlockToSectionY(heightLimitView.GetTopY());
		}

		public static int GetSectionIndexFromBlock(this IHeightLimitView heightLimitView, int blockY) {
			return heightLimitView.GetSectionIndexFromSectionY(SectionPos.BlockToSectionY(blockY));
		}

		public static int GetSectionIndexFromSectionY(this IHeightLimitView heightLimitView, int sectionY) {
			return sectionY - heightLimitView.GetBottomSectionY();
		}

		public static int GetSectionYFromSectionIndex(this IHeightLimitView heightLimitView, int sectionIndex) {
			return sectionIndex + heightLimitView.GetBottomSectionY();
		}

		public static int GetSectionCount(this IHeightLimitView heightLimitView) {
			return heightLimitView.GetTopSectionY() - heightLimitView.GetBottomSectionY() + 1;
		}
	}
}
