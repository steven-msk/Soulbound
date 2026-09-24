namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using System;

	public record RecordMapCodec<T, V>(Field<T, V> field, Func<V, T> construct) : MapCodec<T> {
		public static RecordMapCodec<T, V> Of(Field<T, V> field, Func<V, T> construct) {
			return new RecordMapCodec<T, V>(field, construct);
		}

		public override DataResult<T> Decode(JObject json) {
			return this.field.DecodeFrom(json).Map<T>(this.construct);
		}

		public override void Encode(T value, JObject json) {
			json[this.field.name] = this.field.codec.Encode(this.field.valueSupplier(value));
		}
	}

	public record RecordMapCodec<T, V1, V2>(Field<T, V1> field1, Field<T, V2> field2, Func<V1, V2, T> construct) : MapCodec<T> {
		public static RecordMapCodec<T, V1, V2> Of(Field<T, V1> field1, Field<T, V2> field2, Func<V1, V2, T> construct) {
			return new RecordMapCodec<T, V1, V2>(field1, field2, construct);
		}

		public override DataResult<T> Decode(JObject obj) {
			return RecordCodec<T, V1, V2>.Apply(this.field1.DecodeFrom(obj), this.field2.DecodeFrom(obj), this.construct);
		}

		public override void Encode(T value, JObject target) {
			target[this.field1.name] = this.field1.codec.Encode(this.field1.valueSupplier(value));
			target[this.field2.name] = this.field2.codec.Encode(this.field2.valueSupplier(value));
		}
	}

	public record RecordMapCodec<T, V1, V2, V3>(
		Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3,
		Func<V1, V2, V3, T> construct
	) : MapCodec<T> {
		public static RecordMapCodec<T, V1, V2, V3> Of(Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Func<V1, V2, V3, T> construct) {
			return new RecordMapCodec<T, V1, V2, V3>(field1, field2, field3, construct);
		}

		public override DataResult<T> Decode(JObject json) {
			return RecordCodec<T, V1, V2, V3>.Apply(
				this.field1.DecodeFrom(json), 
				this.field2.DecodeFrom(json), 
				this.field3.DecodeFrom(json), 
				this.construct
			);
		}

		public override void Encode(T value, JObject json) {
			json[this.field1.name] = this.field1.codec.Encode(this.field1.valueSupplier(value));
			json[this.field2.name] = this.field2.codec.Encode(this.field2.valueSupplier(value));
			json[this.field3.name] = this.field3.codec.Encode(this.field3.valueSupplier(value));
		}
	}

	public record RecordMapCodec<T, V1, V2, V3, V4>(
		Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Field<T, V4> field4,
		Func<V1, V2, V3, V4, T> construct
	) : MapCodec<T> {
		public static RecordMapCodec<T, V1, V2, V3, V4> Of(
			Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Field<T, V4> field4,
			Func<V1, V2, V3, V4, T> construct
		) {
			return new RecordMapCodec<T, V1, V2, V3, V4>(field1, field2, field3, field4, construct);
		}

		public override DataResult<T> Decode(JObject json) {
			return RecordCodec<T, V1, V2, V3, V4>.Apply(
				this.field1.DecodeFrom(json),
				this.field2.DecodeFrom(json), 
				this.field3.DecodeFrom(json),
				this.field4.DecodeFrom(json),
				this.construct
			);
		}

		public override void Encode(T value, JObject json) {
			json[this.field1.name] = this.field1.codec.Encode(this.field1.valueSupplier(value));
			json[this.field2.name] = this.field2.codec.Encode(this.field2.valueSupplier(value));
			json[this.field3.name] = this.field3.codec.Encode(this.field3.valueSupplier(value));
			json[this.field4.name] = this.field4.codec.Encode(this.field4.valueSupplier(value));
		}
	}

	public record RecordMapCodec<T, V1, V2, V3, V4, V5>(
		Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Field<T, V4> field4, Field<T, V5> field5,
		Func<V1, V2, V3, V4, V5, T> construct
	) : MapCodec<T> {
		public static RecordMapCodec<T, V1, V2, V3, V4, V5> Of(
			Field<T, V1> field1, Field<T, V2> field2, Field<T, V3> field3, Field<T, V4> field4, Field<T, V5> field5,
			Func<V1, V2, V3, V4, V5, T> construct
		) {
			return new RecordMapCodec<T, V1, V2, V3, V4, V5>(field1, field2, field3, field4, field5, construct);
		}

		public override DataResult<T> Decode(JObject json) {
			return RecordCodec<T, V1, V2, V3, V4, V5>.Apply(
				this.field1.DecodeFrom(json),
				this.field2.DecodeFrom(json),
				this.field3.DecodeFrom(json), 
				this.field4.DecodeFrom(json), 
				this.field5.DecodeFrom(json), 
				this.construct
			);
		}

		public override void Encode(T value, JObject json) {
			json[this.field1.name] = this.field1.codec.Encode(this.field1.valueSupplier(value));
			json[this.field2.name] = this.field2.codec.Encode(this.field2.valueSupplier(value));
			json[this.field3.name] = this.field3.codec.Encode(this.field3.valueSupplier(value));
			json[this.field4.name] = this.field4.codec.Encode(this.field4.valueSupplier(value));
			json[this.field5.name] = this.field5.codec.Encode(this.field5.valueSupplier(value));
		}
	}
}
