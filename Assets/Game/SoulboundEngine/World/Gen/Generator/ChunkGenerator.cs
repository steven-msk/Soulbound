namespace SoulboundEngine.World.Gen.Generator {
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Collection;
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Gen.Feature;
	using SoulboundEngine.World.Level;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public abstract class ChunkGenerator {
		public static readonly Codec<ChunkGenerator> CODEC = new DispatchCodec<ChunkGenerator, RegistryKey<MapCodec<ChunkGenerator>>>(
			RegistryKey<MapCodec<ChunkGenerator>>.Codec(RegistryKeys.CHUNK_GENERATOR),
			"key",
			g => Registries.CHUNK_GENERATOR.GetKey(g.Codec()),
			key => {
				Optional<MapCodec<ChunkGenerator>> entry = Optional<MapCodec<ChunkGenerator>>.Of(Registries.CHUNK_GENERATOR.Get(key)?.GetValue());
				return entry.IsPresent()
					? DataResult<MapCodec<ChunkGenerator>>.Success(entry.GetValue())
					: DataResult<MapCodec<ChunkGenerator>>.Error($"Unknown chunk generator {key}");
			}
		);
		protected readonly BiomeSource biomeSource;
		private readonly Func<RegistryEntry<Biome>, BiomeGenSettings> generationSettingsGetter;
		private List<FeatureGenStep.StepData>? featuresPerStep;

		public ChunkGenerator(BiomeSource biomeSource) 
			: this(biomeSource, biome => biome.GetValue().generationSettings) {
			this.biomeSource = biomeSource;
		}

		public ChunkGenerator(BiomeSource biomeSource, Func<RegistryEntry<Biome>, BiomeGenSettings> genSettingsGetter) {
			this.biomeSource = biomeSource;
			this.generationSettingsGetter = genSettingsGetter;
		}

		/// <summary>
		/// <b>Must return a stable instance.
		/// Reverse lookup will fail if the codec is constructed fresh every call</b>
		/// </summary>
		protected abstract MapCodec<ChunkGenerator> Codec();

		public BiomeGenSettings GetBiomeGenSettings(RegistryEntry<Biome> biome) {
			return this.generationSettingsGetter(biome);
		}

		public virtual Chunk MapBiomes(RandomState randomState, Chunk chunk) {
			chunk.FillBiomesFromNoise(this.biomeSource, randomState.Sampler);
			return chunk;
		}

		public abstract Chunk Fill(RandomState randomState, Chunk chunk);

		public abstract Chunk BuildSurface(RandomState randomState, Chunk chunk);

		public virtual Chunk ApplyDecor(IWorldGenLevel level, RandomState randomState, Chunk chunk) {
			ChunkPos pos = chunk.pos;
			BlockPos origin = new(pos.ToWorldX(0), chunk.GetBottomY());
			List<FeatureGenStep.StepData> featureList = this.featuresPerStep ??= FeatureGenStep.StepData.BuildFeaturesPerStep(
				this.biomeSource.GetPossibleBiomes().ToList(),
				biome => this.generationSettingsGetter(biome).features,
				tryReducingError: true
			);
			WorldGenRandom random = new(RandomProvider.CreateWithUniqueSeed());
			long decorSeed = random.SetDecorationSeed(level.GetSeed(), origin.x);
			HashSet<RegistryEntry<Biome>> possibleBiomes = new();
			for (int cx = 0; cx < ChunkSection.WIDTH; cx++) {
				RegistryEntry<Biome> biome = chunk.GetBiome(cx);
				if (this.biomeSource.GetPossibleBiomes().Contains(biome)) {
					possibleBiomes.Add(biome);
				}
			}
			int featureStepCount = featureList.Count;

			try {
				Registry<PlacedFeature> featureRegistry = level.GetRegistries().GetOrThrow(RegistryKeys.PLACED_FEATURE);
				int generationSteps = Math.Max(FeatureGenStep.GetOrdinal().Length, featureStepCount);

				for (int stepIndex = 0; stepIndex < generationSteps; stepIndex++) {
					if (stepIndex < featureStepCount) {
						HashSet<int> possibleFeaturesThisStep = new();

						foreach (RegistryEntry<Biome> biome in possibleBiomes) {
							List<IRegistryEntryList<PlacedFeature>> featuresInBiome = this.generationSettingsGetter(biome).features;
							if (stepIndex < featuresInBiome.Count) {
								IRegistryEntryList<PlacedFeature> featuresInBiomeThisStep = featuresInBiome[stepIndex];
								FeatureGenStep.StepData stepData = featureList[stepIndex];
								featuresInBiomeThisStep.ForEach(feature => possibleFeaturesThisStep.Add(stepData.indexOf[feature]));
							}
						}

						int numberOfFeaturesInStep = possibleFeaturesThisStep.Count;
						int[] indexArray = possibleFeaturesThisStep.ToArray();
						Array.Sort(indexArray);
						FeatureGenStep.StepData stepFeatureData = featureList[stepIndex];

						for (int featureIndex = 0; featureIndex < numberOfFeaturesInStep; featureIndex++) {
							int globalIndexOfFeature = indexArray[featureIndex];
							PlacedFeature feature = stepFeatureData.features[globalIndexOfFeature].GetValue();
							random.SetFeatureSeed(decorSeed, globalIndexOfFeature, stepIndex);

							try {
								feature.Generate(level, this, random, origin);
							} catch (Exception e) {
								Logger.LogFatal(e, "Unexpected exception during feature placement");
								throw e;
							}
						}
					}
				}
			} catch (Exception e) {
				Logger.LogFatal(e, "Unexpected exception during decor generation");
				throw e;
			}

			return chunk;
		}

		public abstract int GetMinGenY();

		public abstract int GetGenHeight();

		public abstract int GetBaseHeight(RandomState randomState, int x, IHeightLimitView heightLimit);

		public int GetFirstFreeHeight(RandomState randomState, int x, IHeightLimitView heightLimit) {
			return this.GetBaseHeight(randomState, x, heightLimit);
		}

		public int GetFirstOccupiedHeight(RandomState randomState, int x, IHeightLimitView heightLimit) {
			return this.GetBaseHeight(randomState, x, heightLimit) - 1;
		}
	}
}
