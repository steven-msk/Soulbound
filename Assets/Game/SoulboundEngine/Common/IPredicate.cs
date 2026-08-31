namespace SoulboundEngine.Common {
	using System;

	public interface IPredicate<T> {
		public IPredicate<T> And(IPredicate<T> other) {
			return Of(v => this.Test(v) && other.Test(v));
		}

		public IPredicate<T> Or(IPredicate<T> other) {
			return Of(v => this.Test(v) || other.Test(v));
		}

		public IPredicate<T> Negate() => Not(this);

		public static IPredicate<T> Not(IPredicate<T> target) {
			return Of(v => !target.Test(v));
		}

		bool Test(T value);

		public static IPredicate<T> Of(Predicate<T> predicate) {
			return new DelegateImpl(predicate);
		}

		private class DelegateImpl : IPredicate<T> {
			private readonly Predicate<T> predicate;

			public DelegateImpl(Predicate<T> predicate) {
				this.predicate = predicate;
			}

			public bool Test(T value) => this.predicate(value);
		}
	}
}
