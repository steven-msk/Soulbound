namespace SoulboundEngine.World.Gen.HeightProvider {
	using SoulboundEngine.Common.Math.Random;

	public interface IHeightProvider {
		int Sample(IRandom random, WorldGenContext context);
	}
}
