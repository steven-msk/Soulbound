namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using System;
	using System.Collections.Generic;

	public class MultiNoiseBiomeSourceParamList {
		public static readonly Codec<MultiNoiseBiomeSourceParamList> DIRECT_CODEC = Preset.CODEC.Xmap(p => new MultiNoiseBiomeSourceParamList(p, Registries.BIOME), l => l.preset);
		public static readonly Codec<RegistryEntry<MultiNoiseBiomeSourceParamList>> CODEC = RegistryEntry<MultiNoiseBiomeSourceParamList>.GetCodec(Registries.MULTI_NOISE_BIOME_SOURCE_PARAMETER_LIST);
		public static readonly RegistryKey<MultiNoiseBiomeSourceParamList> DEFAULT = Create("default");
		private readonly Preset preset;
		private readonly Climate.ParameterList<RegistryEntry<Biome>> parameters;

		private static RegistryKey<MultiNoiseBiomeSourceParamList> Create(string id) {
			return RegistryKey<MultiNoiseBiomeSourceParamList>.Of(RegistryKeys.MULTI_NOISE_BIOME_SOURCE_PARAMETER_LIST, Identifier.Of(id));
		}

		public static MultiNoiseBiomeSourceParamList Init(RegistryBootstrapContext context, Registry<MultiNoiseBiomeSourceParamList> registry) {
			IRegistryEntryLookup<Biome> biomes = context.Lookup(RegistryKeys.BIOME);
			return Registry<MultiNoiseBiomeSourceParamList>.Register(registry, DEFAULT, FromPreset(Preset.DEFAULT, biomes));
		}

		private static MultiNoiseBiomeSourceParamList FromPreset(Preset preset, IRegistryEntryLookup<Biome> biomes) {
			return new MultiNoiseBiomeSourceParamList(preset, biomes);
		}

		public MultiNoiseBiomeSourceParamList(Preset preset, IRegistryEntryLookup<Biome> registryLookup) {
			this.preset = preset;
			this.parameters = preset.provider.Apply(registryLookup.GetOrThrow);
		}

		public Climate.ParameterList<RegistryEntry<Biome>> GetParameters() => this.parameters;

		public record Preset(Identifier id, Preset.ISourceProvider provider) {
			private static readonly Dictionary<Identifier, Preset> BY_ID = new();
			public static readonly Codec<Preset> CODEC = Identifier.CODEC.FlatXmap(
				encode: p => p.id,
				decode: id => BY_ID.TryGetValue(id, out Preset preset)
					? DataResult<Preset>.Success(preset)
					: DataResult<Preset>.Error($"Unknown preset: {id}")
			);
			public static readonly Preset DEFAULT = Register(Identifier.Of("default"), new DefaultBiomeMapSourceProvider());

			private static Preset Register(Identifier id, ISourceProvider sourceProvider) {
				Preset preset = new(id, sourceProvider);
				BY_ID.Add(id, preset);
				return preset;
			}

			public interface ISourceProvider {
				Climate.ParameterList<T> Apply<T>(Func<RegistryKey<Biome>, T> lookup);
			}

			private sealed class DefaultBiomeMapSourceProvider : ISourceProvider {
				public Climate.ParameterList<T> Apply<T>(Func<RegistryKey<Biome>, T> lookup) {
					Climate.ParameterList<T>.Builder builder = new();
					new BiomeMapBuilder().AddBiomes((biome, point) => {
						builder.Add(point, lookup(biome));
					});
					return builder.Build();
				}
			}
		}
	}
}
