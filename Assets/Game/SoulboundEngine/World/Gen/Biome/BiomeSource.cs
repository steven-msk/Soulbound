namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Common;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public abstract class BiomeSource : IBiomeResolver {
		public static readonly Codec<BiomeSource> CODEC = new DispatchCodec<BiomeSource, RegistryKey<MapCodec<BiomeSource>>>(
			RegistryKey<MapCodec<BiomeSource>>.Codec(RegistryKeys.BIOME_SOURCE),
			"key",
			s => Registries.BIOME_SOURCE.GetKey(s.Codec()),
			key => {
				Optional<MapCodec<BiomeSource>> entry = Optional<MapCodec<BiomeSource>>.Of(Registries.BIOME_SOURCE.Get(key)?.GetValue());
				return entry.IsPresent()
					? DataResult<MapCodec<BiomeSource>>.Success(entry.GetValue())
					: DataResult<MapCodec<BiomeSource>>.Error($"Unknown biome source {key}");
			}
		);
		private HashSet<RegistryEntry<Biome>>? possibleBiomes;
		private readonly Func<HashSet<RegistryEntry<Biome>>> possibleBiomesSupplier;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
		public static MapCodec<BiomeSource> Init(RegistryBootstrapContext context, Registry<MapCodec<BiomeSource>> registry) {
			Registry<MapCodec<BiomeSource>>.Register(registry, "flat", SingleBiomeSource.CODEC);
			return Registry<MapCodec<BiomeSource>>.Register(registry, "multi_noise", MultiNoiseBiomeSource.CODEC);
		}

		protected BiomeSource() {
			this.possibleBiomesSupplier = () => this.possibleBiomes ??= this.CollectPossibleBiomes().ToHashSet();
		}

		/// <summary>
		/// <b>Must return a stable instance.
		/// Reverse lookup will fail if the codec is constructed fresh every call</b>
		/// </summary>
		protected abstract MapCodec<BiomeSource> Codec();
		
		protected abstract IEnumerable<RegistryEntry<Biome>> CollectPossibleBiomes();

		public HashSet<RegistryEntry<Biome>> GetPossibleBiomes() => this.possibleBiomesSupplier();

		public abstract RegistryEntry<Biome> GetNoiseBiome(int x, int y, Climate.Sampler sampler);
	}
}
