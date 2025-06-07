using System;
using System.Diagnostics.CodeAnalysis;

public readonly struct Optional<T> {
	private readonly T value;

	private Optional(T value) {
		this.value = value;
	}

	public static Optional<T> Empty() => new Optional<T>(default);

	public static Optional<T> Of([AllowNull] T value) {
		return value is null ? Empty() : new Optional<T>(value);
	}

	public T GetValue() {
		if (IsPresent()) { 
			return value; 
		}
		throw new InvalidOperationException("No value present");
	}

	public bool IsPresent() {
		return value is not null;
	}

	public void IfPresent(Action<T> method) {
		if (IsPresent()) { 
			method.Invoke(value); 
		}
	}

	public void IfPresent(Func<object> method) {
		if (IsPresent()) { 
			method.Invoke(); 
		}
	}

	public T OrElse(T other) {
		return IsPresent() ? value : other;
	}

	public T OrElseGet(Func<T> method) {
		return IsPresent() ? value : method.Invoke();
	}

	public T OrElseThrow(Func<Exception> method) {
		return IsPresent() ? value : throw method.Invoke();
	}

	public Optional<TU> Map<TU>(Func<T, TU> method) {
		return IsPresent() ? new Optional<TU>(method.Invoke(value)) : default;
	}
}