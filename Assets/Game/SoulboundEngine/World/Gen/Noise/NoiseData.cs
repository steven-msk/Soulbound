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
				this.Register(NoiseTypes.ROUGHNESS, -1, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d);
				this.Register(NoiseTypes.BASE_CAVE, -2, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d);
				return this.Register(NoiseTypes.SHAPE, -5, 1.5d, 1.0d, 2.0d, 2.0d, 2.0d, 1.0d, 1.0d);
			}

			private NormalNoise.Parameters Create(int firstOctave, params double[] octaveMultipliers) {
				return NormalNoise.Parameters.Of(firstOctave, octaveMultipliers);
			}

			private NormalNoise.Parameters Register(RegistryKey<NormalNoise.Parameters> key, int firstOctave, params double[] octaveMultipliers) {
				return this.Register(key, this.Create(firstOctave, octaveMultipliers));
			}

			private NormalNoise.Parameters Register(RegistryKey<NormalNoise.Parameters> key, NormalNoise.Parameters parameters) {
				return Registry<NormalNoise.Parameters>.Register(this.registry, key, parameters);
			}
		}
	}
}
