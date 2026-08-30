namespace SoulboundEngine.Common.Patterns {
	using System;

	public readonly struct Optional<T> {
		private readonly T value;

		private Optional(T value) {
			this.value = value;
		}

		public static Optional<T> Empty() => new(default);

		public static Optional<T> Of(T value) {
			return value is null ? Empty() : new Optional<T>(value);
		}

		public static Optional<T> CastFrom<V>(Optional<V> other) where V : T {
			return CastFrom(other, v => v);
		}

		public static Optional<T> CastFrom<V>(Optional<V> other, Func<V, T> valueFunction) where V : T {
			return other.IsEmpty() ? Empty() : Of(valueFunction(other.GetValue()));
		}

		public static Optional<V> CastTo<V>(Optional<T> other) where V : T {
			return CastTo(other, v => (V)v);
		}

		public static Optional<V> CastTo<V>(Optional<T> other, Func<T, V> valueFunction) where V : T {
			return other.IsEmpty() ? Optional<V>.Empty() : Optional<V>.Of(valueFunction(other.GetValue()));
		} 

		public T GetValue() {
			return this.IsPresent() ? this.value : throw new InvalidOperationException("No value present");
		}

		public bool IsPresent() => this.value is not null;

		public bool IsEmpty() => this.value is null;

		public void IfPresent(Action<T> method) {
			if (this.IsPresent()) { 
				method.Invoke(this.value); 
			}
		}

		public void IfPresent(Func<object> method) {
			if (this.IsPresent()) { 
				method.Invoke(); 
			}
		}

		public T OrElse(T other) {
			return this.IsPresent() ? this.value : other;
		}

		public T OrElseGet(Func<T> method) {
			return this.IsPresent() ? this.value : method.Invoke();
		}

		public T OrElseThrow(Func<Exception> method) {
			return this.IsPresent() ? this.value : throw method.Invoke();
		}

		public T OrElseThrow() => this.OrElseThrow(() => new InvalidOperationException("Empty optional"));

		public Optional<TU> Map<TU>(Func<T, TU> method) {
			return this.IsPresent() ? new Optional<TU>(method.Invoke(this.value)) : default;
		}
	}

#nullable enable

	public static class OptionalExtras {
		public static Optional<T> OfUnmanaged<T>(T? value) where T : unmanaged {
			return value.HasValue ? Optional<T>.Of(value.Value) : Optional<T>.Empty(); 
		}
	}
}
