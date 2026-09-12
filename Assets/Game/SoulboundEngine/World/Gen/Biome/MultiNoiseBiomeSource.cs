namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class MultiNoiseBiomeSource : BiomeSource {
		private readonly Climate.ParameterList<RegistryEntry<Biome>> parameters;

		private MultiNoiseBiomeSource(Climate.ParameterList<RegistryEntry<Biome>> parameters) {
			this.parameters = parameters;
		}

		public static MultiNoiseBiomeSource FromParameterList(Climate.ParameterList<RegistryEntry<Biome>> parameters) {
			return new MultiNoiseBiomeSource(parameters);
		}

		public static MultiNoiseBiomeSource FromPreset(RegistryEntry<MultiNoiseBiomeSourceParamList> parameters) {
			return new MultiNoiseBiomeSource(parameters.GetValue().GetParameters());
		}

		public override RegistryEntry<Biome> GetNoiseBiome(int x, int y, Climate.Sampler sampler) {
			Climate.TargetPoint target = sampler.Sample(x, y);
			return this.parameters.FindBruteForce(target);
		}

		protected override IEnumerable<RegistryEntry<Biome>> CollectPossibleBiomes() {
			return this.parameters.entries.Select(v => v.value);
		}
	}
}
