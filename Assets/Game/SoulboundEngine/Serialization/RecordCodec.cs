namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common.Patterns;
	using System;
	using System.Collections.Generic;

	public record RecordCodec<T, V>(Field<T, V> field, Func<V, T> construct) : Codec<T> {
		public static RecordCodec<T, V> Of(Field<T, V> field, Func<V, T> construct) {
			return new RecordCodec<T, V>(field, construct);
		}

		public override DataResult<T> Decode(JToken json) {
			DataResult<V> fieldResult = this.field.DecodeFrom(json);
			return fieldResult.Map<T>(this.construct);
		}

		public override JToken Encode(T value) {
			return new JObject() {
				[this.field.name] = this.field.codec.Encode(this.field.valueSupplier(value))
			};
		}
	}

	public record RecordCodec<T, V1, V2>(Field<T, V1> field1, Field<T, V2> field2, Func<V1, V2, T> construct) : Codec<T> {
		public static RecordCodec<T, V1, V2> Of(Field<T, V1> field1, Field<T, V2> field2, Func<V1, V2, T> construct) {
			return new RecordCodec<T, V1, V2>(field1, field2, construct);
		}

		public override DataResult<T> Decode(JToken json) {
			return Apply(this.field1.DecodeFrom(json), this.field2.DecodeFrom(json), this.construct);
		}

		public override JToken Encode(T value) {
			return new JObject() {
				[this.field1.name] = this.field1.codec.Encode(this.field1.valueSupplier(value))
				[this.field2.name] = this.field2.codec.Encode(this.field2.valueSupplier(value))
			};
		}

		public static DataResult<T> Apply(DataResult<V1> r1, DataResult<V2> r2, Func<V1, V2, T> construct) {
			List<string> errors = new();
			Optional<V1> v1 = r1.ResultOrPartial(errors.Add);
			Optional<V2> v2 = r2.ResultOrPartial(errors.Add);

			if (errors.Count > 0) {
				string message = string.Join("; ", errors);
				return v1.IsPresent() && v2.IsPresent()
					? DataResult<T>.Error(message, construct(v1.GetValue(), v2.GetValue()))
					: DataResult<T>.Error(message);
			}
			return DataResult<T>.Success(construct(v1.GetValue(), v2.GetValue()));
		}
	}

	public record RecordCodec<T, V1, V2, V3>(
		Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, 
		Func<V1, V2, V3, T> construct
	) : Codec<T> {
		public static RecordCodec<T, V1, V2, V3> Of(Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Func<V1, V2, V3, T> construct) {
			return new RecordCodec<T, V1, V2, V3>(field1, field2, field3, construct);
		}

		public override DataResult<T> Decode(JToken json) {
			return Apply(this.field1.DecodeFrom(json), this.field2.DecodeFrom(json), this.field3.DecodeFrom(json), this.construct);
		}

		public override JToken Encode(T value) {
			return new JObject() {
				[this.field1.name] = this.field1.codec.Encode(this.field1.valueSupplier(value)),
				[this.field2.name] = this.field2.codec.Encode(this.field2.valueSupplier(value)),
				[this.field3.name] = this.field3.codec.Encode(this.field3.valueSupplier(value))
			};
		}

		public static DataResult<T> Apply(DataResult<V1> r1, DataResult<V2> r2, DataResult<V3> r3, Func<V1, V2, V3, T> construct) {
			List<string> errors = new();
			Optional<V1> v1 = r1.ResultOrPartial(errors.Add);
			Optional<V2> v2 = r2.ResultOrPartial(errors.Add);
			Optional<V3> v3 = r3.ResultOrPartial(errors.Add);

			if (errors.Count > 0) {
				string message = string.Join("; ", errors);
				return v1.IsPresent() && v2.IsPresent() && v3.IsEmpty()
					? DataResult<T>.Error(message, construct(v1.GetValue(), v2.GetValue(), v3.GetValue()))
					: DataResult<T>.Error(message);
			}
			return DataResult<T>.Success(construct(v1.GetValue(), v2.GetValue(), v3.GetValue()));
		}
	}

	public record RecordCodec<T, V1, V2, V3, V4>(
		Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Field<T, V4> field4,
		Func<V1, V2, V3, V4, T> construct
	) : Codec<T> {
		public static RecordCodec<T, V1, V2, V3, V4> Of(
			Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Field<T, V4> field4,
			Func<V1, V2, V3, V4, T> construct
		) {
			return new RecordCodec<T, V1, V2, V3, V4>(field1, field2, field3, field4, construct);
		}

		public override DataResult<T> Decode(JToken json) {
			return Apply(this.field1.DecodeFrom(json), this.field2.DecodeFrom(json), this.field3.DecodeFrom(json), this.field4.DecodeFrom(json), this.construct);
		}

		public override JToken Encode(T value) {
			return new JObject() {
				[this.field1.name] = this.field1.codec.Encode(this.field1.valueSupplier(value)),
				[this.field2.name] = this.field2.codec.Encode(this.field2.valueSupplier(value)),
				[this.field3.name] = this.field3.codec.Encode(this.field3.valueSupplier(value)),
				[this.field4.name] = this.field4.codec.Encode(this.field4.valueSupplier(value))
			};
		}

		public static DataResult<T> Apply(DataResult<V1> r1, DataResult<V2> r2, DataResult<V3> r3, DataResult<V4> r4, Func<V1, V2, V3, V4, T> construct) {
			List<string> errors = new();
			Optional<V1> v1 = r1.ResultOrPartial(errors.Add);
			Optional<V2> v2 = r2.ResultOrPartial(errors.Add);
			Optional<V3> v3 = r3.ResultOrPartial(errors.Add);
			Optional<V4> v4 = r4.ResultOrPartial(errors.Add);

			if (errors.Count > 0) {
				string message = string.Join("; ", errors);
				return v1.IsPresent() && v2.IsPresent() && v3.IsPresent() && v4.IsPresent()
					? DataResult<T>.Error(message, construct(v1.GetValue(), v2.GetValue(), v3.GetValue(), v4.GetValue()))
					: DataResult<T>.Error(message);
			}
			return DataResult<T>.Success(construct(v1.GetValue(), v2.GetValue(), v3.GetValue(), v4.GetValue()));
		}
	}

	public record RecordCodec<T, V1, V2, V3, V4, V5>(
		Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Field<T, V4> field4, Field<T, V5> field5,
		Func<V1, V2, V3, V4, V5, T> construct
	) : Codec<T> {
		public static RecordCodec<T, V1, V2, V3, V4, V5> Of(
			Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Field<T, V4> field4, Field<T, V5> field5,
			Func<V1, V2, V3, V4, V5, T> construct
		) {
			return new RecordCodec<T, V1, V2, V3, V4, V5>(field1, field2, field3, field4, field5, construct);
		}

		public override DataResult<T> Decode(JToken json) {
			return Apply(this.field1.DecodeFrom(json), this.field2.DecodeFrom(json), this.field3.DecodeFrom(json), this.field4.DecodeFrom(json), this.field5.DecodeFrom(json), this.construct);
		}

		public override JToken Encode(T value) {
			return new JObject() {
				[this.field1.name] = this.field1.codec.Encode(this.field1.valueSupplier(value)),
				[this.field2.name] = this.field2.codec.Encode(this.field2.valueSupplier(value)),
				[this.field3.name] = this.field3.codec.Encode(this.field3.valueSupplier(value)),
				[this.field4.name] = this.field4.codec.Encode(this.field4.valueSupplier(value)),
				[this.field5.name] = this.field5.codec.Encode(this.field5.valueSupplier(value))
			};
		}

		public static DataResult<T> Apply(DataResult<V1> r1, DataResult<V2> r2, DataResult<V3> r3, DataResult<V4> r4, DataResult<V5> r5, Func<V1, V2, V3, V4, V5, T> construct) {
			List<string> errors = new();
			Optional<V1> v1 = r1.ResultOrPartial(errors.Add);
			Optional<V2> v2 = r2.ResultOrPartial(errors.Add);
			Optional<V3> v3 = r3.ResultOrPartial(errors.Add);
			Optional<V4> v4 = r4.ResultOrPartial(errors.Add);
			Optional<V5> v5 = r5.ResultOrPartial(errors.Add);

			if (errors.Count > 0) {
				string message = string.Join("; ", errors);
				return v1.IsPresent() && v2.IsPresent() && v3.IsPresent() && v4.IsPresent() && v5.IsPresent()
					? DataResult<T>.Error(message, construct(v1.GetValue(), v2.GetValue(), v3.GetValue(), v4.GetValue(), v5.GetValue()))
					: DataResult<T>.Error(message);
			}
			return DataResult<T>.Success(construct(v1.GetValue(), v2.GetValue(), v3.GetValue(), v4.GetValue(), v5.GetValue()));
		}
	}
}
