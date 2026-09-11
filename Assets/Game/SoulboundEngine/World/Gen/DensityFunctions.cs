namespace SoulboundEngine.World.Gen.Biome {
	using System;

	public sealed class DensityFunctions {
		public static IDensityFunction Zero() => Constant.ZERO;

		public static IDensityFunction Map(IDensityFunction function, Mapped.Type type) {
			return new Mapped(type, function);
		}

		public interface ITransformer : IDensityFunction {
			IDensityFunction input { get; }

			double Transform(double input);

			double IDensityFunction.Compute(IContext context) {
				return this.Transform(this.input.Compute(context));
			}

			void IDensityFunction.FillArray(double[] output, IContextProvider contextProvider) {
				this.input.FillArray(output, contextProvider);
				for (int i = 0; i < output.Length; i++) {
					output[i] = this.Transform(output[i]);
				}
			}
		}

		public record Noise(IDensityFunction.NoiseEntry noise) : IDensityFunction {
			public double Compute(IDensityFunction.IContext context) {
				return this.noise.Get(context.blockX, context.blockY, 0.0d);
			}

			public void FillArray(double[] output, IDensityFunction.IContextProvider contextProvider) {
				contextProvider.FillAllDirectly(output, this);
			}

			public IDensityFunction MapAll(IDensityFunction.IVisitor visitor) {
				return visitor.Apply(new Noise(visitor.VisitNoise(this.noise)));
			}
		}

		public record Constant(double value) : IDensityFunction.ISimple, IDensityFunction {
			public static readonly Constant ZERO = new(0.0d);

			void IDensityFunction.FillArray(double[] output, IDensityFunction.IContextProvider contextProvider) {
				Array.Fill(output, this.value);
			}

			public double Compute(IDensityFunction.IContext context) {
				return this.value;
			}
		}

		public record Mapped(Mapped.Type type, IDensityFunction input) : ITransformer, IDensityFunction {
			public IDensityFunction MapAll(IDensityFunction.IVisitor visitor) {
				return new Mapped(this.type, this.input.MapAll(visitor));
			}

			public double Transform(double input) => Transform(this.type, input);

			private static double Transform(Type type, double input) {
				return type switch {
					Type.ABS => Math.Abs(input),
					Type.SQUARE => input * input,
					Type.CUBE => input * input * input,
					Type.HALF_NEGATIVE => input > 0.0d ? input : input * 0.5d,
					Type.QUARTER_NEGATIVE => input > 0.0d ? input : input * 0.25d,
					Type.INVERT => 1.0d / input,
					Type.SQUEEZE => Squeeze(input),
					_ => throw new NotImplementedException()
				};
			}

			private static double Squeeze(double input) {
				double c = Math.Clamp(input, -1.0d, 1.0d);
				return c / 2.0d - c * c * c / 24.0d;
			}

			public enum Type {
				ABS,
				SQUARE,
				CUBE,
				HALF_NEGATIVE,
				QUARTER_NEGATIVE,
				INVERT,
				SQUEEZE
			}
		}		

		public record Clamp(IDensityFunction input, double minValue, double maxValue) : ITransformer {
			public IDensityFunction MapAll(IDensityFunction.IVisitor visitor) {
				return new Clamp(this.input.MapAll(visitor), this.minValue, this.maxValue);
			}

			public double Transform(double input) {
				return Math.Clamp(input, this.minValue, this.maxValue);
			}
		}
	}
}
