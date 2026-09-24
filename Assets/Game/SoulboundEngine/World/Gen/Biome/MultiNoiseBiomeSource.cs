namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class MultiNoiseBiomeSource : BiomeSource {
		public new static readonly MapCodec<BiomeSource> CODEC = RecordMapCodec<BiomeSource, RegistryEntry<MultiNoiseBiomeSourceParamList>>.Of(
			Field.Required<BiomeSource, RegistryEntry<MultiNoiseBiomeSourceParamList>>("parameters", MultiNoiseBiomeSourceParamList.CODEC, s => ((MultiNoiseBiomeSource)s).parameters),
			FromPreset
		);
		private readonly RegistryEntry<MultiNoiseBiomeSourceParamList> parameters;

		private MultiNoiseBiomeSource(RegistryEntry<MultiNoiseBiomeSourceParamList> parameters) {
			this.parameters = parameters;
		}

		public static MultiNoiseBiomeSource FromPreset(RegistryEntry<MultiNoiseBiomeSourceParamList> parameters) {
			return new MultiNoiseBiomeSource(parameters);
		}

		public override RegistryEntry<Biome> GetNoiseBiome(int x, int y, Climate.Sampler sampler) {
			Climate.TargetPoint target = sampler.Sample(x, y);
			return this.parameters.GetValue().GetParameters().FindBruteForce(target);
		}

		protected override MapCodec<BiomeSource> Codec() => CODEC;

		protected override IEnumerable<RegistryEntry<Biome>> CollectPossibleBiomes() {
			return this.parameters.GetValue().GetParameters().entries.Select(v => v.value);
		}
	}
}
