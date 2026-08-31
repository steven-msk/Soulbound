namespace SoulboundEngine.Serialization {
	using SoulboundEngine.Common;
	using System;

#nullable enable

	public readonly struct DataResult<R> {
		private readonly Optional<R> result;
		private readonly bool isSuccess;
		private readonly string? message;

		private DataResult(bool isSuccess, Optional<R> result, string? message) {
			this.isSuccess = isSuccess;
			this.result = result;
			this.message = message;
		}

		public static DataResult<R> Success(R result) {
			return new DataResult<R>(true, Optional<R>.Of(result), null);
		}

		public static DataResult<R> Error(Func<string> message) => Error(message());

		public static DataResult<R> Error(string message) {
			return new DataResult<R>(false, Optional<R>.Empty(), message);
		}

		public static DataResult<R> Error(Func<string> message, R partialResult) => Error(message(), partialResult);

		public static DataResult<R> Error(string message, R partialResult) {
			return new DataResult<R>(false, Optional<R>.Of(partialResult), message);
		}

		public Optional<R> Result() => this.IsSuccess() ? this.result : Optional<R>.Empty();

		public DataResult<R> Error() => new(false, this.result, this.message);

		public bool HasResultOrPartial() => this.result.IsPresent();

		public Optional<R> ResultOrPartial(Action<string>? onError = null) {
			if (this.IsError()) onError?.Invoke(this.message!);
			return this.result;
		}

		public string GetMessage() => this.message ?? string.Empty;

		public bool IsError() => !this.isSuccess;

		public bool IsSuccess() => this.isSuccess;

		public R GetOrThrow() => this.GetOrThrow(message => new InvalidOperationException(message));

		public R GetPartialOrThrow() => this.GetPartialOrThrow(message => new InvalidOperationException(message));

		public R GetOrThrow<E>(Func<string, E> exceptionSupplier) where E : Exception {
			return this.IsSuccess() ? this.result.GetValue() : throw exceptionSupplier(this.GetMessage());
		}

		public R GetPartialOrThrow<E>(Func<string, E> exceptionSupplier) where E : Exception {
			return this.result.IsPresent() ? this.result.GetValue() : throw exceptionSupplier(this.GetMessage());
		}

		public DataResult<T> Map<V, T>(Func<V, T> function) where V : R {
			return this.IsSuccess()
				? new DataResult<T>(true, Optional<T>.Of(function((V)this.result.GetValue()!)), this.message)
				: new DataResult<T>(false, this.result.IsEmpty() ? Optional<T>.Empty() : Optional<T>.Of(function((V)this.result.GetValue()!)), this.message); 
		}

		public DataResult<T> Map<T>(Func<R, T> function) => this.Map<R, T>(function);

		public T MapOrElse<V, T>(Func<V, T> successFunction, Func<DataResult<V>, T> errorFunction) where V : R {
			return this.IsSuccess()
				? successFunction((V)this.result.GetValue()!)
				: errorFunction(CastTo<V>(this));
		}

		public DataResult<V> IfSuccess<V>(Action<V> ifSuccess) where V : R {
			if (this.IsSuccess()) ifSuccess((V)this.result.GetValue()!);
			return CastTo<V>(this);
		}

		public DataResult<R> IfSuccess(Action<R> ifSuccess) => this.IfSuccess<R>(ifSuccess);

		public DataResult<V> IfError<V>(Action<DataResult<V>> ifError) where V : R {
			if (this.IsError()) ifError(CastTo<V>(this));
			return CastTo<V>(this);
		}

		public DataResult<R> IfError(Action<DataResult<R>> ifError) => this.IfError<R>(ifError);

		public DataResult<R> PromotePartial(Action<string> onError) {
			if (this.IsError()) onError(this.GetMessage());
			return this.result.IsEmpty() ? this : new DataResult<R>(true, this.result, this.message);
		}

		public DataResult<T> FlatMap<T>(Func<R, DataResult<T>> function) {
			if (this.result.IsEmpty()) return new DataResult<T>(this.isSuccess, Optional<T>.Empty(), this.message);

			DataResult<T> other = function(this.result.GetValue()!);
			return this.IsSuccess() ? other : new DataResult<T>(false, other.result, CombineErrorMessages(this.message, other.message));
		}

		private static string CombineErrorMessages(string? a, string? b) {
			return a != null && b != null ? $"{a}; {b}" : a ?? b ?? string.Empty;
		}

		public DataResult<R> SetPartial(Func<R> partial) => this.SetPartial(partial());

		public DataResult<R> SetPartial(R partial) {
			return this.IsSuccess() ? this : new DataResult<R>(this.isSuccess, Optional<R>.Of(partial), this.message);
		}

		private static DataResult<V> CastTo<V>(DataResult<R> result) where V : R {
			return new DataResult<V>(result.isSuccess, Optional<R>.CastTo<V>(result.result), result.message);
		}

		private static DataResult<R> CastFrom<V>(DataResult<V> result) where V : R {
			return new DataResult<R>(result.isSuccess, Optional<R>.CastFrom(result.result), result.message);
		}

		public override string ToString() {
			return $"dataResult.{(this.isSuccess ? "success" : "error")}['{this.GetMessage()}'{(this.result.IsPresent() ? $": {this.result.GetValue()}" : "")}]";
		}
	}
}
