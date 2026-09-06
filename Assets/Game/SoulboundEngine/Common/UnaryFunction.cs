namespace SoulboundEngine.Common {
	public delegate T UnaryFunction<T>(T value);

	public interface IUnaryFunction<T> {
		T Apply(T value);

		public static IUnaryFunction<T> Of(UnaryFunction<T> function) {
			return new DelegateImpl(function);
		}

		private sealed class DelegateImpl : IUnaryFunction<T> {
			private readonly UnaryFunction<T> function;

			public DelegateImpl(UnaryFunction<T> function) {
				this.function = function;
			}

			public T Apply(T value) => this.function(value);
		}
	}
}
