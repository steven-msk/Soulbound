namespace SoulboundEngine.Common.Math.Random {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;

	public interface IPositionalRandomFactory {
		IRandom FromHashOf(string s);

		IRandom FromSeed(long seed);

		IRandom At(int x, int y, int z);
	}

	public static class PositionalRandomFactoryDefaults {
		public static IRandom At(this IPositionalRandomFactory factory, BlockPos blockPos) {
			return factory.At(blockPos.x, blockPos.y, 0);
		}

		public static IRandom FromHashOf(this IPositionalRandomFactory factory, Identifier id) {
			return factory.FromHashOf(id.ToString());
		}
	}
}
