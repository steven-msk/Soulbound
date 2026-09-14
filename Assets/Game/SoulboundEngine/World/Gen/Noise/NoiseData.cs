namespace SoulboundEngine.World.Gen.Noise {
	using SoulboundEngine.Registry;

	public static class NoiseData {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
		public static NormalNoise.Parameters Init(RegistryBootstrapContext context, Registry<NormalNoise.Parameters> registry) {
			return new Bootstrap(registry, context).Register();
		}

		private sealed class Bootstrap {
			private readonly Registry<NormalNoise.Parameters> registry;
			private readonly RegistryBootstrapContext context;

			public Bootstrap(Registry<NormalNoise.Parameters> registry, RegistryBootstrapContext context) {
				this.registry = registry;
				this.context = context;
			}

			public NormalNoise.Parameters Register() {
				this.Register(NoiseTypes.ROUGHNESS, -3, 1.0d, 1.0d, 1.0d);
				this.Register(NoiseTypes.BASE_CAVE, -2, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d);
				return this.Register(NoiseTypes.SHAPE, -5, 1.0d, 1.0d, 2.0d, 2.0d, 2.0d, 1.0d, 1.0d);
			}

			private NormalNoise.Parameters Register(RegistryKey<NormalNoise.Parameters> key, int firstOctave, params double[] octaveMultipliers) {
				return Registry<NormalNoise.Parameters>.Register(this.registry, key, NormalNoise.Parameters.Of(firstOctave, octaveMultipliers));
			}
		}
	}
}
