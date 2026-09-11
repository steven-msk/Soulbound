namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using System;

	public class MultiNoiseBiomeSourceParamList {
		private readonly Preset preset;
		private readonly Climate.ParameterList<RegistryEntry<Biome>> parameters;

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
