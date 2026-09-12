namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Registry;
	using System;

#nullable enable

	public interface IDensityFunction {
		double Compute(IContext context);

		void FillArray(double[] output, IContextProvider contextProvider);

		IDensityFunction MapAll(IVisitor visitor);

		public interface IContext {
			int blockX { get; }
			int blockY { get; }
		}

		public interface IContextProvider {
			IContext ForIndex(int index);

			void FillAllDirectly(double[] output, IDensityFunction function);
		}

		public interface IVisitor {
			IDensityFunction Apply(IDensityFunction input);

			virtual NoiseEntry VisitNoise(NoiseEntry noise) => noise;

			public static IVisitor Of(Func<IDensityFunction, IDensityFunction> apply) {
				return new DelegateImpl(apply, null);
			}

			public static IVisitor Of(Func<IDensityFunction, IDensityFunction> apply, Func<NoiseEntry, NoiseEntry> visitNoise) {
				return new DelegateImpl(apply, visitNoise);
			}

			private sealed record DelegateImpl(Func<IDensityFunction, IDensityFunction> apply, Func<NoiseEntry, NoiseEntry>? visitNoise) : IVisitor {
				public IDensityFunction Apply(IDensityFunction input) {
					return this.apply(input);
				}

				NoiseEntry IVisitor.VisitNoise(NoiseEntry noise) {
					return this.visitNoise?.Invoke(noise) ?? noise;
				}
			}
		}

		public interface ISimple : IDensityFunction {
			void IDensityFunction.FillArray(double[] output, IContextProvider contextProvider) {
				contextProvider.FillAllDirectly(output, this);
			}

			IDensityFunction IDensityFunction.MapAll(IVisitor visitor) {
				return visitor.Apply(this);
			}
		}

		public sealed record SinglePointContext(int blockX, int blockY) : IContext;

		public sealed record NoiseEntry(RegistryEntry<NormalNoise.Parameters> parameters, NormalNoise? noise) {
			public double Get(double x, double y, double z) {
				return this.noise?.Get(x, y, z) ?? 0.0d;
			}
		}
	}

	public static class DensityFunctionDefaults {
		public static IDensityFunction Clamp(this IDensityFunction function, double min, double max) {
			return new DensityFunctions.Clamp(function, min, max);
		}

		public static IDensityFunction Abs(this IDensityFunction function) => DensityFunctions.Map(function, DensityFunctions.Mapped.Type.ABS);

		public static IDensityFunction Square(this IDensityFunction function) => DensityFunctions.Map(function, DensityFunctions.Mapped.Type.SQUARE);

		public static IDensityFunction Cube(this IDensityFunction function) => DensityFunctions.Map(function, DensityFunctions.Mapped.Type.CUBE);

		public static IDensityFunction HalfNegative(this IDensityFunction function) => DensityFunctions.Map(function, DensityFunctions.Mapped.Type.HALF_NEGATIVE);

		public static IDensityFunction QuarterNegative(this IDensityFunction function) => DensityFunctions.Map(function, DensityFunctions.Mapped.Type.QUARTER_NEGATIVE);

		public static IDensityFunction Invert(this IDensityFunction function) => DensityFunctions.Map(function, DensityFunctions.Mapped.Type.INVERT);

		public static IDensityFunction Squeeze(this IDensityFunction function) => DensityFunctions.Map(function, DensityFunctions.Mapped.Type.SQUEEZE);
	}
}
