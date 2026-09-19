namespace SoulboundEngine.World.Gen.Feature {
	using SoulboundEngine.Registry;

	public abstract class Feature {
		public static readonly Feature<NoOpFeature.Config> NO_OP = Register("no_op", new NoOpFeature());

		private static Feature<C> Register<C>(string id, Feature<C> feature) where C : IFeatureConfig {
			return Registry<Feature>.Register(Registries.FEATURE, RegistryKey<Feature>.Of(RegistryKeys.FEATURE, Identifier.Of(id)), feature);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
		public static Feature Init(RegistryBootstrapContext context, Registry<Feature> registry) => NO_OP;
	}

	public abstract class Feature<C> : Feature where C : IFeatureConfig {
	}
}
