namespace SoulboundEngine.UnityClient.Editor.Tools.WorldGen {
    using SoulboundEngine.Common.Collection;
    using SoulboundEngine.Registry;
    using SoulboundEngine.World;
    using SoulboundEngine.World.Chunk;
    using SoulboundEngine.World.Gen;
    using SoulboundEngine.World.Gen.Biome;
    using SoulboundEngine.World.Gen.Feature;
    using SoulboundEngine.World.Gen.Generator;
    using SoulboundEngine.World.Gen.Noise;
    using SoulboundEngine.World.Level;
    using System;
    using System.Collections.Generic;
    using System.Linq;

#nullable enable

    public sealed class EditorWorldGenContext {
		private readonly HashSet<Identifier> loaded = new();
		private readonly List<(Identifier registry, Func<object> loader)> loaders = new();
		private readonly Dictionary<Identifier, IRegistry> requiredRegistries = new();
		private RegistryBootstrapContext? registryBootstrapContext;
		private readonly Registry<MultiNoiseBiomeSourceParamList> multiNoiseBiomeSourceParamLists;
		private readonly Registry<NormalNoise.Parameters> noises;
		private readonly Registry<NoiseGeneratorSettings> noiseGeneratorSettings;
        private readonly Registry<LevelType> levelTypes;
        private readonly IRegistryManager registries;
        private readonly IHeightLimitView heightLimit;
        private LevelType levelType = null!;

		public EditorWorldGenContext() {
			_ = Registries.ROOT_IDENTIFIER;
			this.AddRequiredRegistry(RegistryKeys.BIOME, Biome.Init);
			this.multiNoiseBiomeSourceParamLists = this.AddRequiredRegistry(RegistryKeys.MULTI_NOISE_BIOME_SOURCE_PARAMETER_LIST, MultiNoiseBiomeSourceParamList.Init);
			this.noises = this.AddRequiredRegistry(RegistryKeys.NOISE, NoiseData.Init);
			this.AddRequiredRegistry(RegistryKeys.DENSITY_FUNCTION, NoiseRouter.Init);
			this.noiseGeneratorSettings = this.AddRequiredRegistry(RegistryKeys.NOISE_SETTINGS, NoiseGeneratorSettings.Init);
            this.AddRequiredRegistry(RegistryKeys.FEATURE, Feature.Init);
            this.AddRequiredRegistry(RegistryKeys.CONFIGURED_FEATURE, ConfiguredFeature.Init);
            this.AddRequiredRegistry(RegistryKeys.PLACED_FEATURE, PlacedFeature.Init);
            this.levelTypes = this.AddRequiredRegistry(RegistryKeys.LEVEL_TYPE, LevelType.Init);

            this.registries = IRegistryManager.Of(this.requiredRegistries.Values.ToList());
            this.heightLimit = IHeightLimitView.Create(Level.DEFAULT_MIN_Y, Level.DEFAULT_WORLD_HEIGHT);
        }

        public void Init() {
			foreach ((Identifier registry, Func<object> loader) in this.loaders) {
				if (loader() == null) {
					Logger.LogError("Unable to load registry '{}'", registry);
				}
				if (!this.loaded.Add(registry)) {
					Logger.LogError("Registry is already loaded: '{}'", registry);
				}
			}
			this.requiredRegistries.Values.ForEach(r => r.Freeze());
            this.levelType = this.levelTypes.GetOrThrow(LevelType.DEFAULT).GetValue();
        }

        public NoiseLevelChunkGenerator CreateChunkGenerator(RegistryKey<NoiseGeneratorSettings> noiseGeneratorSettings, RegistryKey<MultiNoiseBiomeSourceParamList> noiseParameterPreset) {
			return new NoiseLevelChunkGenerator(
				MultiNoiseBiomeSource.FromPreset(this.multiNoiseBiomeSourceParamLists.GetOrThrow(noiseParameterPreset)),
				this.noiseGeneratorSettings.GetOrThrow(noiseGeneratorSettings)
			);
		}

		public RandomState CreateRandomStateWithOverrides(
			RegistryKey<NoiseGeneratorSettings> noiseGeneratorSettings, 
			long seed,
			Dictionary<RegistryKey<NormalNoise.Parameters>, NormalNoise.Parameters> overrides
		) {
			return RandomState.Create(
				this.noiseGeneratorSettings.GetOrThrow(noiseGeneratorSettings).GetValue(),
				new OverridableNoiseLookup(this.noises, this.noises, overrides),
				seed
			);
		}

		public IWorldGenLevel CreateLevel(long seed, int chunkCount, int chunkStartX) {
			return new EditorWorldGenLevel(this.levelType, seed, chunkCount, this.registries, index => {
                ChunkPos chunkPos = new(index + chunkStartX);
				return new EditorWorldGenChunk(chunkPos, this.heightLimit, () => new BlockStateContainer(ChunkSection.WIDTH, ChunkSection.HEIGHT));
			});
		}

		public Registry<NormalNoise.Parameters> Noises => this.noises;

		private Registry<T> AddRequiredRegistry<T>(RegistryKey<Registry<T>> registryKey, Func<RegistryBootstrapContext, Registry<T>, object> bootstrapper) where T : class {
			Registry<T> registry = this.requiredRegistries.Put(registryKey.value, new Registry<T>(registryKey));
			this.loaders.Add((registryKey.value, () => bootstrapper(this.registryBootstrapContext ??= new RegistryBootstrapContext(this.GetRegistryLookup()), registry)));
			return registry;
		}

		public IRegistryLookup GetRegistryLookup() => IRegistryLookup.Of(this.requiredRegistries.GetOrThrow);
	}
}
