namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Collection;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen.Biome;
	using System;
	using System.Collections.Generic;
	using static DensityFunctions;

	public record NoiseRouter(IDensityFunction[] densityFunctions) {
		private static readonly Dictionary<Climate.ParameterType, RegistryKey<IDensityFunction>> PARAMETER_DENSITY_KEYS = new();
		private static readonly Dictionary<RegistryKey<IDensityFunction>, Func<IRegistryEntryLookup<NormalNoise.Parameters>, IDensityFunction>> FUNCTION_FACTORIES = new();
		public static readonly RegistryKey<IDensityFunction> ZERO = CreateKey("zero");
		public static readonly RegistryKey<IDensityFunction> SHAPE = CreateParameterKey("shape", Climate.ParameterType.SHAPE, CreateShapeFunction);

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
			return Registry<IDensityFunction>.Register(registry, ZERO, DensityFunctions.Zero());
		}

		public static NoiseRouter CreateDefault(IRegistryEntryLookup<IDensityFunction> densityFunctions, IRegistryEntryLookup<NormalNoise.Parameters> noises) {
			return ParameterTypeMapped(parameter => GetFunction(densityFunctions, PARAMETER_DENSITY_KEYS.GetOrThrow(parameter)));
		}

		public static NoiseRouter Zero() {
			return ParameterTypeMapped(_ => DensityFunctions.Zero());
		}

		private static NoiseRouter ParameterTypeMapped(Func<Climate.ParameterType, IDensityFunction> densityFunctionFactory) {
			return new NoiseRouter(Climate.ParameterType.Map(densityFunctionFactory));
		}

		private static IDensityFunction GetFunction(IRegistryEntryLookup<IDensityFunction> functions, RegistryKey<IDensityFunction> key) {
			return functions.GetOrThrow(key).GetValue();
		}

		private static IDensityFunction CreateShapeFunction(IRegistryEntryLookup<NormalNoise.Parameters> noises) {
			return CreateNoise(noises.GetOrThrow(NoiseTypes.SHAPE)) * 80.0d;
		}

		public NoiseRouter MapAll(IDensityFunction.IVisitor visitor) {
			IDensityFunction[] newFunctions = new IDensityFunction[this.densityFunctions.Length];
			for (int i = 0; i < this.densityFunctions.Length; i++) {
				newFunctions[i] = this.densityFunctions[i].MapAll(visitor);
			}
			return new NoiseRouter(newFunctions);
		}

		public IDensityFunction GetParameterNoise(Climate.ParameterType parameterType) {
			return parameterType.Get(this.densityFunctions);
		}
	}
}
