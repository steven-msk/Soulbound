namespace SoulboundEngine.World.Gen.Noise {
	using SoulboundEngine.Common.Collection;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen.Biome;
	using SoulboundEngine.World.Gen.Function;
	using System;
	using System.Collections.Generic;
	using static Function.DensityFunctions;

	public record NoiseRouter(
		IDensityFunction[] parameterFunctions, 
		IDensityFunction terrainHeight,
		IDensityFunction finalTerrain
	) {
		private static readonly Dictionary<Climate.ParameterType, RegistryKey<IDensityFunction>> PARAMETER_DENSITY_KEYS = new();
		private static readonly Dictionary<RegistryKey<IDensityFunction>, Func<IRegistryEntryLookup<NormalNoise.Parameters>, IDensityFunction>> FUNCTION_FACTORIES = new();
		public static readonly RegistryKey<IDensityFunction> ZERO = CreateKey("zero");
		public static readonly RegistryKey<IDensityFunction> SHAPE = CreateParameterKey("shape", Climate.ParameterType.SHAPE, CreateShapeFunction);
		public static readonly RegistryKey<IDensityFunction> TERRAIN_HEIGHT = CreateKey("terrain_height");
		public static readonly RegistryKey<IDensityFunction> FINAL_TERRAIN = CreateKey("final_terrain");

		private static RegistryKey<IDensityFunction> CreateKey(string id) {
			return RegistryKey<IDensityFunction>.Of(RegistryKeys.DENSITY_FUNCTION, Identifier.Of(id));
		}

		private static RegistryKey<IDensityFunction> CreateParameterKey(string id, Climate.ParameterType parameterType, Func<IRegistryEntryLookup<NormalNoise.Parameters>, IDensityFunction> functionFactory) {
			RegistryKey<IDensityFunction> key = CreateKey(id);
			PARAMETER_DENSITY_KEYS.Add(parameterType, key);
			FUNCTION_FACTORIES.Add(key, functionFactory);
			return key;
		}

		public static IDensityFunction Init(RegistryBootstrapContext context, Registry<IDensityFunction> registry) {
			IRegistryEntryLookup<NormalNoise.Parameters> noises = context.Lookup(RegistryKeys.NOISE);
			Climate.ParameterType.Map(PARAMETER_DENSITY_KEYS.GetOrThrow).ForEach(densityKey => {
				Registry<IDensityFunction>.Register(registry, densityKey, FUNCTION_FACTORIES.GetOrThrow(densityKey)(noises));
			});
			Registry<IDensityFunction>.Register(registry, TERRAIN_HEIGHT, CreateTerrainHeight(noises));
			Registry<IDensityFunction>.Register(registry, FINAL_TERRAIN, CreateFinalTerrain(registry, noises));
			return Registry<IDensityFunction>.Register(registry, ZERO, DensityFunctions.Zero());
		}

		public static NoiseRouter CreateDefault(IRegistryEntryLookup<IDensityFunction> densityFunctions, IRegistryEntryLookup<NormalNoise.Parameters> noises) {
			return ParameterTypeMapped(densityFunctions, parameter => GetFunction(densityFunctions, PARAMETER_DENSITY_KEYS.GetOrThrow(parameter)));
		}

		public static NoiseRouter Zero() {
			return new NoiseRouter(MapParameters(_ => DensityFunctions.Zero()), DensityFunctions.Zero(), DensityFunctions.Zero());
		}

		private static IDensityFunction[] MapParameters(Func<Climate.ParameterType, IDensityFunction> factory) {
			return Climate.ParameterType.Map(factory);
		}

		private static NoiseRouter ParameterTypeMapped(IRegistryEntryLookup<IDensityFunction> functions, Func<Climate.ParameterType, IDensityFunction> densityFunctionFactory) {
			return new NoiseRouter(
				parameterFunctions: Climate.ParameterType.Map(densityFunctionFactory), 
				terrainHeight: GetFunction(functions, TERRAIN_HEIGHT),
				finalTerrain: GetFunction(functions, FINAL_TERRAIN)
			);
		}

		private static IDensityFunction GetFunction(IRegistryEntryLookup<IDensityFunction> functions, RegistryKey<IDensityFunction> key) {
			return functions.GetOrThrow(key).GetValue();
		}

		private static IDensityFunction CreateShapeFunction(IRegistryEntryLookup<NormalNoise.Parameters> noises) {
			return CreateNoise(noises.GetOrThrow(NoiseTypes.SHAPE));
		}

		private static IDensityFunction CreateFinalTerrain(IRegistryEntryLookup<IDensityFunction> functions, IRegistryEntryLookup<NormalNoise.Parameters> noises) {
			IDensityFunction terrainHeight = GetFunction(functions, TERRAIN_HEIGHT);
			IDensityFunction terrainDensity = terrainHeight - Y;

			IDensityFunction caveNoise = CreateNoise(noises.GetOrThrow(NoiseTypes.BASE_CAVE));
			const double CAVE_SIZE = 0.02d;
			IDensityFunction caveDensity = Abs(caveNoise) - CAVE_SIZE;

			return Min(terrainDensity, caveDensity);
		}

		private static IDensityFunction CreateTerrainHeight(IRegistryEntryLookup<NormalNoise.Parameters> noises) {
			IDensityFunction shape = CreateNoise(noises.GetOrThrow(NoiseTypes.SHAPE));
			IDensityFunction roughness = CreateNoise(noises.GetOrThrow(NoiseTypes.ROUGHNESS));
			IDensityFunction hilliness = Max(shape, 0.0d);

			IDensityFunction baseHeight = shape * 80.0d;
			IDensityFunction extraAmp = hilliness * shape * 600.0d;
			IDensityFunction roughnessTerm = hilliness * roughness * 40.0d;
			return baseHeight + roughnessTerm + extraAmp;
		}

		public NoiseRouter MapAll(IDensityFunction.IVisitor visitor) {
			IDensityFunction[] newFunctions = new IDensityFunction[this.parameterFunctions.Length];
			for (int i = 0; i < this.parameterFunctions.Length; i++) {
				newFunctions[i] = this.parameterFunctions[i].MapAll(visitor);
			}
			return new NoiseRouter(newFunctions, this.terrainHeight.MapAll(visitor), this.finalTerrain.MapAll(visitor));
		}

		public IDensityFunction GetParameterNoise(Climate.ParameterType parameterType) {
			return parameterType.Get(this.parameterFunctions);
		}
	}
}
