namespace SoulboundEngine.World.Gen.Biome {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen.Feature;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public class BiomeGenSettings {
		public static readonly BiomeGenSettings EMPTY = new(new List<IRegistryEntryList<PlacedFeature>>());
		public List<IRegistryEntryList<PlacedFeature>> features { get; }
		private HashSet<PlacedFeature>? featureSet;

		public BiomeGenSettings(List<IRegistryEntryList<PlacedFeature>> features) {
			this.features = features;
		}

		public static RegistryBackedBuilder RegistryBacked(IRegistryEntryLookup<PlacedFeature> placedFeatures) {
			return new RegistryBackedBuilder(placedFeatures);	
		}

		public static PlainBuilder Plain() => new();

		public bool HasFeature(PlacedFeature feature) {
			return (this.featureSet ??= this.features.SelectMany(f => f
						.Select(e => e.GetValue()))
					.ToHashSet())
				.Contains(feature);
		}

		public class RegistryBackedBuilder : PlainBuilder {
			private readonly IRegistryEntryLookup<PlacedFeature> placedFeatures;

			public RegistryBackedBuilder(IRegistryEntryLookup<PlacedFeature> placedFeatures) {
				this.placedFeatures = placedFeatures;
			}

			public RegistryBackedBuilder AddFeature(FeatureGenStep step, RegistryKey<PlacedFeature> feature) {
				this.AddFeature(step.ordinalIndex, this.placedFeatures.GetOrThrow(feature));
				return this;
			}
		}

		public class PlainBuilder {
			private readonly List<List<RegistryEntry<PlacedFeature>>> features = new();

			public PlainBuilder AddFeature(FeatureGenStep step, RegistryEntry<PlacedFeature> feature) {
				return this.AddFeature(step.ordinalIndex, feature);
			}

			public PlainBuilder AddFeature(int index, RegistryEntry<PlacedFeature> feature) {
				this.AddRequiredFeatureSteps(index);
				this.features[index].Add(feature);
				return this;
			}

			private void AddRequiredFeatureSteps(int index) {
				while (this.features.Count <= index) {
					this.features.Add(new List<RegistryEntry<PlacedFeature>>());
				}
			}

			public BiomeGenSettings Build() {
				return new BiomeGenSettings(this.features.Select(IRegistryEntryList<PlacedFeature>.CreateDirect).ToList());
			}
		}
	}
}
