namespace SoulboundEngine.Common {
	using System;

	public interface IFunction<I, R> {
		R Apply(I input);

		public static IFunction<I, R> Of(Func<I, R> func) => new DelegateImpl(func);

		private sealed class DelegateImpl : IFunction<I, R> {
			private readonly Func<I, R> func;

			public DelegateImpl(Func<I, R> func) {
				this.func = func;
			}

			public R Apply(I input) => this.func(input);
		}
	}

	public interface IFunction<I1, I2, R> {
		R Apply(I1 input1, I2 input2);

		public static IFunction<I1, I2, R> Of(Func<I1, I2, R> func) => new DelegateImpl(func);

		private sealed class DelegateImpl : IFunction<I1, I2, R> {
			private readonly Func<I1, I2, R> func;

			public DelegateImpl(Func<I1, I2, R> func) {
				this.func = func;
			}

			public R Apply(I1 input1, I2 input2) => this.func(input1, input2);
		}
	}

	public interface IFunction<I1, I2, I3, R> {
		R Apply(I1 input1, I2 input2, I3 input3);

		public static IFunction<I1, I2, I3, R> Of(Func<I1, I2, I3, R> func) => new DelegateImpl(func);

		private sealed class DelegateImpl : IFunction<I1, I2, I3, R> {
			private readonly Func<I1, I2, I3, R> func;

			public DelegateImpl(Func<I1, I2, I3, R> func) {
				this.func = func;
			}

			public R Apply(I1 input1, I2 input2, I3 input3) => this.func(input1, input2, input3);
		}
	}
}
