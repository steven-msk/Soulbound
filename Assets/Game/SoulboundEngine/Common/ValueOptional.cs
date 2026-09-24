namespace SoulboundEngine.Common {
	using System;

	public readonly struct ValueOptional<T> where T : struct {
		private readonly T? value;

		private ValueOptional(T? value) {
			this.value = value;
		}

		public static ValueOptional<T> Of(T? value) => new(value);

		public static ValueOptional<T> Of(T nonNull) => new(nonNull);

		public static ValueOptional<T> Empty() => Of(null);

		public T GetValue() {
			return this.IsPresent() ? this.value.Value : throw new InvalidOperationException("No value present");
		}

		public T? GetAsIs() => this.value;

		public bool IsPresent() => this.value.HasValue;

		public bool IsEmpty() => !this.IsPresent();

		public void IfPresent(Action<T> method) {
			if (this.IsPresent()) {
				method.Invoke(this.value.Value);
			}
		}

		public void IfPresent(Func<object> method) {
			if (this.IsPresent()) {
				method.Invoke();
			}
		}

		public T OrElse(T other) {
			return this.IsPresent() ? this.value.Value : other;
		}

		public T OrElseGet(Func<T> method) {
			return this.IsPresent() ? this.value.Value : method.Invoke();
		}

		public T OrElseThrow(Func<Exception> method) {
			return this.IsPresent() ? this.value.Value : throw method.Invoke();
		}

		public T OrElseThrow() => this.OrElseThrow(() => new InvalidOperationException("Empty optional"));

		public ValueOptional<TU> Map<TU>(Func<T, TU> method) where TU : struct {
			return this.IsPresent() ? new ValueOptional<TU>(method.Invoke(this.value.Value)) : default;
		}
	}
}
