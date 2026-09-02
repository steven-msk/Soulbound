namespace SoulboundEngine.Common {
	using System;

	public readonly struct UnmanagedOptional<T> where T : unmanaged {
		private readonly T? value;

		private UnmanagedOptional(T? value) {
			this.value = value;
		}

		public static UnmanagedOptional<T> Of(T? value) => new(value);

		public static UnmanagedOptional<T> Of(T nonNull) => new(nonNull);

		public static UnmanagedOptional<T> Empty() => Of(null);

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

		public UnmanagedOptional<TU> Map<TU>(Func<T, TU> method) where TU : unmanaged {
			return this.IsPresent() ? new UnmanagedOptional<TU>(method.Invoke(this.value.Value)) : default;
		}
	}
}
