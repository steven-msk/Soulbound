namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen.Feature;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public readonly struct FeatureGenStep {
		private static readonly List<FeatureGenStep> INDEXED_VALUES = new();
		public static readonly FeatureGenStep VEGETATION = Register("vegetation", 0);
		public static readonly FeatureGenStep ORES = Register("ores", 1);
		private static FeatureGenStep[]? ordinalSteps;
		public readonly int ordinalIndex;
		public readonly string serializedName;

		public FeatureGenStep(string serializedName, int ordinalIndex) {
			this.serializedName = serializedName;
			this.ordinalIndex = ordinalIndex;
		}

		private static FeatureGenStep Register(string serializedName, int ordinalIndex) {
			return Register(new FeatureGenStep(serializedName, ordinalIndex));
		}

		private static FeatureGenStep Register(FeatureGenStep step) {
			INDEXED_VALUES.Insert(step.ordinalIndex, step);
			return step;
		}

		public static FeatureGenStep[] GetOrdinal() => ordinalSteps ??= INDEXED_VALUES.ToArray();

		public sealed record StepData(List<RegistryEntry<PlacedFeature>> features, Dictionary<RegistryEntry<PlacedFeature>, int> indexOf) {
			private static readonly IComparer<FeatureData> FEATURE_DATA_COMPARER = Comparer<FeatureData>.Create((a, b) => {
				int stepCompare = a.step.CompareTo(b.step);
				return stepCompare != 0 ? stepCompare : a.featureIndex.CompareTo(b.featureIndex);
			});

			public StepData(List<RegistryEntry<PlacedFeature>> features) 
				: this(features, BuildIndex(features)) {
			}

			private static Dictionary<RegistryEntry<PlacedFeature>, int> BuildIndex(List<RegistryEntry<PlacedFeature>> features) {
				Dictionary<RegistryEntry<PlacedFeature>, int> index = new();
				for (int i = 0; i < features.Count; i++) index[features[i]] = i;
				return index;
			}

			public static List<StepData> BuildFeaturesPerStep<T>(
				IReadOnlyList<T> featureSources,
				Func<T, IReadOnlyList<IRegistryEntryList<PlacedFeature>>> featureGetter,
				bool tryReducingError
			) {
				Dictionary<RegistryEntry<PlacedFeature>, int> featureIndex = new();
				int nextFeatureIndex = 0;
				SortedDictionary<FeatureData, SortedSet<FeatureData>> edges = new(FEATURE_DATA_COMPARER);
				int maxStep = 0;

				foreach (T source in featureSources) {
					List<FeatureData> featureList = new();
					IReadOnlyList<IRegistryEntryList<PlacedFeature>> featuresForStep = featureGetter(source);
					maxStep = Math.Max(maxStep, featuresForStep.Count);

					for (int i = 0; i < featuresForStep.Count; i++) {
						foreach (RegistryEntry<PlacedFeature> feature in featuresForStep[i]) {
							if (!featureIndex.TryGetValue(feature, out int index)) {
								featureIndex[feature] = index = nextFeatureIndex++;
							}
							featureList.Add(new FeatureData(index, i, feature));
						}
					}

					for (int i = 0; i < featureList.Count; i++) {
						if (!edges.TryGetValue(featureList[i], out SortedSet<FeatureData>? dependents)) {
							edges[featureList[i]] = dependents = new SortedSet<FeatureData>(FEATURE_DATA_COMPARER);
						}
						if (i < featureList.Count - 1) {
							dependents.Add(featureList[i + 1]);
						}
					}
				}

				SortedSet<FeatureData> discovered = new(FEATURE_DATA_COMPARER);
				SortedSet<FeatureData> currentlyVisiting = new(FEATURE_DATA_COMPARER);
				List<FeatureData> sortedFeatures = new();

				static bool DepthFirstSearch(
					IReadOnlyDictionary<FeatureData, SortedSet<FeatureData>> edges,
					SortedSet<FeatureData> discovered,
					SortedSet<FeatureData> currentlyVisiting,
					Action<FeatureData> output,
					FeatureData start
				) {
					if (discovered.Contains(start)) return false;
					if (!currentlyVisiting.Add(start)) return true;

					if (edges.TryGetValue(start, out SortedSet<FeatureData>? neighbors)) {
						foreach (FeatureData neighbor in neighbors) {
							if (DepthFirstSearch(edges, discovered, currentlyVisiting, output, neighbor)) return true;
						}
					}

					currentlyVisiting.Remove(start);
					discovered.Add(start);
					output(start);
					return false;
				}

				foreach (FeatureData feature in edges.Keys) {
					if (currentlyVisiting.Count != 0) {
						throw new InvalidOperationException("Feature sort DFS state broken: iteration finished with a non-empty in-progress vertex set");
					}

					if (!discovered.Contains(feature) && DepthFirstSearch(edges, discovered, currentlyVisiting, sortedFeatures.Add, feature)) {
						if (!tryReducingError) {
							throw new InvalidOperationException("Feature order cycle found");
						}

						List<T> reducedSources = new(featureSources);
						int lastSize;
						do {
							lastSize = reducedSources.Count;
							for (int i = reducedSources.Count - 1; i >= 0; i--) {
								T removed = reducedSources[i];
								reducedSources.RemoveAt(i);
								try {
									BuildFeaturesPerStep(reducedSources, featureGetter, false);
									reducedSources.Insert(i, removed);
								} catch (InvalidOperationException) {
								}
							}
						} while (lastSize != reducedSources.Count);

						string culprits = string.Join(", ", reducedSources.Select(s => s?.ToString() ?? "?"));
						throw new InvalidOperationException($"Feature order cycle found, involved sources: {culprits}");
					}
				}

				sortedFeatures.Reverse();

				List<StepData> result = new();
				for (int step = 0; step < maxStep; step++) {
					int s = step;
					List<RegistryEntry<PlacedFeature>> featuresInStep = sortedFeatures
						.Where(f => f.step == s)
						.Select(f => f.feature)
						.ToList();
					result.Add(new StepData(featuresInStep));
				}
				return result;
			}

			private sealed record FeatureData(int featureIndex, int step, RegistryEntry<PlacedFeature> feature);

		}
	}
}
