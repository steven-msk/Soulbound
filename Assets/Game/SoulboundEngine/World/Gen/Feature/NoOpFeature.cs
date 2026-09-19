namespace SoulboundEngine.World.Gen.Feature {
	public class NoOpFeature : Feature<NoOpFeature.Config> {


		public class Config : IFeatureConfig {
			public static readonly Config INSTANCE = new();

			private Config() { }
		}
	}
}
