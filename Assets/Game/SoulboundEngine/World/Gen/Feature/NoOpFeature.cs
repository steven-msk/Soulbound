namespace SoulboundEngine.World.Gen.Feature {
	public class NoOpFeature : Feature<NoOpFeature.Config> {
		public override bool Generate(FeaturePlaceContext<Config> context) {
			return true;
		}

		public class Config : IFeatureConfig {
			public static readonly Config INSTANCE = new();

			private Config() { }
		}
	}
}
