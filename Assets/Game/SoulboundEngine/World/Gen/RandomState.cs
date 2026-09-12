namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen.Biome;
	using System.Collections.Generic;

	public sealed class RandomState {
		private readonly IPositionalRandomFactory random;
		private readonly IRegistryEntryLookup<NormalNoise.Parameters> noises;
		private readonly NoiseRouter noiseRouter;
		private readonly Climate.Sampler sampler;
		private readonly Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise> noiseInstances = new();
		private readonly Dictionary<Identifier, IPositionalRandomFactory> positionalRandoms = new();

		private RandomState(long seed, NoiseGeneratorSettings noiseSettings, IRegistryEntryLookup<NormalNoise.Parameters> noises) {
			this.random = noiseSettings.randomFactory(seed).ForkPositional();
			this.noises = noises;

			IDensityFunction.IVisitor noiseWiringVisitor = IDensityFunction.IVisitor.Of(
				visitNoise: noise => {
					RegistryEntry<NormalNoise.Parameters> parameters = noise.parameters;
					NormalNoise instance = this.GetOrCreateNoise(parameters.GetKey());
					return new IDensityFunction.NoiseEntry(parameters, instance);
				},
				apply: f => f
			);
			this.noiseRouter = noiseSettings.noiseRouter.MapAll(noiseWiringVisitor);
			this.sampler = new Climate.Sampler(this.noiseRouter.densityFunctions);
		}

		public static RandomState Create(NoiseGeneratorSettings noiseSettings, IRegistryEntryLookup<NormalNoise.Parameters> noises, long seed) {
			return new RandomState(seed, noiseSettings, noises);
		}

		public NormalNoise GetOrCreateNoise(RegistryKey<NormalNoise.Parameters> noise) {
			if (this.noiseInstances.TryGetValue(noise, out NormalNoise result)) {
				return result;
			}
			result = NoiseTypes.Instantiate(this.noises, this.random, noise);
			this.noiseInstances[noise] = result;
			return result;
		}

		public IPositionalRandomFactory GetOrCreateRandomFactory(Identifier id) {
			if (this.positionalRandoms.TryGetValue(id, out IPositionalRandomFactory factory)) {
				return factory;
			}
			factory = this.random.FromHashOf(id).ForkPositional();
			this.positionalRandoms[id] = factory;
			return factory;
		}

		public NoiseRouter Router => this.noiseRouter;

		public Climate.Sampler Sampler => this.sampler;
	}
}
