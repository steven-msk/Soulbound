namespace SoulboundEngine.World.Gen.Function {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen.Noise;
	using System;

	public sealed class DensityFunctions {
		public static IDensityFunction Zero() => Constant.ZERO;

		public static IDensityFunction Map(IDensityFunction function, Mapped.Type type) {
			return new Mapped(type, function);
		}

		public static IDensityFunction CreateNoise(RegistryEntry<NormalNoise.Parameters> parameters) {
			return new Noise(new IDensityFunction.NoiseEntry(parameters, null));
		}

		public static IDensityFunction Add(IDensityFunction first, IDensityFunction second) {
			return new OperationBasedFunction(IOperationBasedFunction.Type.ADD, first, second);
		}

		public static IDensityFunction Mul(IDensityFunction first, IDensityFunction second) {
			return new OperationBasedFunction(IOperationBasedFunction.Type.MULTIPLY, first, second);
		}

		public static IDensityFunction Min(IDensityFunction first, IDensityFunction second) {
			return new OperationBasedFunction(IOperationBasedFunction.Type.MIN, first, second);
		}

		public static IDensityFunction Max(IDensityFunction first, IDensityFunction second) {
			return new OperationBasedFunction(IOperationBasedFunction.Type.MAX, first, second);
		}

		public static IDensityFunction Max(IDensityFunction function, double value) {
			return Max(function, Const(value));
		}

		public static IDensityFunction Min(IDensityFunction function, double value) {
			return Min(function, Const(value));
		}

		public static IDensityFunction Const(double value) => new Constant(value);

		public static IDensityFunction Abs(IDensityFunction function) => function.Abs();

		public static IDensityFunction Square(IDensityFunction function) => function.Square();

		public static IDensityFunction Cube(IDensityFunction function) => function.Cube();

		public static IDensityFunction HalfNegative(IDensityFunction function) => function.HalfNegative();

		public static IDensityFunction QuarterNegative(IDensityFunction function) => function.QuarterNegative();

		public static IDensityFunction Invert(IDensityFunction function) => function.Invert();

		public static IDensityFunction Squeeze(IDensityFunction function) => function.Squeeze();

		public static IDensityFunction MapFromUnitTo(IDensityFunction function, double min, double max) {
			double middle = (min + max) * 0.5d;
			double factor = (max - min) * 0.5d;
			return Add(Const(middle), Mul(Const(factor), function));
		}

		public static IDensityFunction X => new XCoordinate();

		public static IDensityFunction Y => new YCoordinate();

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

		public interface IOperationBasedFunction : IDensityFunction {
			IDensityFunction second { get; }
			IDensityFunction first { get; }
			Type type { get; }

			public enum Type {
				ADD,
				MULTIPLY,
				MIN,
				MAX
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

		private sealed record OperationBasedFunction(IOperationBasedFunction.Type type, IDensityFunction first, IDensityFunction second) : IOperationBasedFunction {
			public double Compute(IDensityFunction.IContext context) {
				double value1 = this.first.Compute(context);
				return this.type switch {
					IOperationBasedFunction.Type.ADD => value1 + this.GetSecond(context),
					IOperationBasedFunction.Type.MULTIPLY => value1 == 0.0d ? 0.0d : value1 * this.GetSecond(context),
					IOperationBasedFunction.Type.MIN => Math.Min(value1, this.GetSecond(context)),
					IOperationBasedFunction.Type.MAX => Math.Max(value1, this.GetSecond(context)),
					_ => throw new NotImplementedException()
				};
			}

			private double GetSecond(IDensityFunction.IContext context) => this.second.Compute(context);

			private double GetFirst(IDensityFunction.IContext context) => this.first.Compute(context);

			public void FillArray(double[] output, IDensityFunction.IContextProvider contextProvider) {
				this.first.FillArray(output, contextProvider);
				switch (this.type) {
					case IOperationBasedFunction.Type.ADD:
						double[] value2 = new double[output.Length];
						this.second.FillArray(value2, contextProvider);
						for (int i = 0; i < output.Length; i++) {
							output[i] += value2[i];
						}
						break;
					case IOperationBasedFunction.Type.MULTIPLY:
						for (int i = 0; i < output.Length; i++) {
							output[i] = output[i] == 0.0d ? 0.0d : this.second.Compute(contextProvider.ForIndex(i)) * output[i];
						}
						break;
					case IOperationBasedFunction.Type.MIN:
						for (int i = 0; i < output.Length; i++) {
							output[i] = Math.Min(output[i], this.second.Compute(contextProvider.ForIndex(i)));
						}
						break;
					case IOperationBasedFunction.Type.MAX:
						for (int i = 0; i < output.Length; i++) {
							output[i] = Math.Max(output[i], this.second.Compute(contextProvider.ForIndex(i)));
						}
						break;
				}
			}

			public IDensityFunction MapAll(IDensityFunction.IVisitor visitor) {
				return visitor.Apply(new OperationBasedFunction(this.type, this.first.MapAll(visitor), this.second.MapAll(visitor)));
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

		public record YCoordinate : IDensityFunction.ISimple {
			public double Compute(IDensityFunction.IContext context) {
				return context.blockY;
			}
		}

		public record XCoordinate : IDensityFunction.ISimple {
			public double Compute(IDensityFunction.IContext context) {
				return context.blockX;
			}
		}
	}
}
