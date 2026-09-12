namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using System;

	public class MultiNoiseBiomeSourceParamList {
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
			public static readonly Preset DEFAULT = new(Identifier.Of("default"), new DefaultBiomeMapSourceProvider());

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
