namespace SoulboundEngine.World.Gen.Biome {
	public interface IDensityFunction {

		float Compute(IContext context);
		
		public interface IContext {
			int blockX { get; }
			int blockY { get; }
		}

		public sealed record SinglePointContext(int blockX, int blockY) : IContext;
	}
}
