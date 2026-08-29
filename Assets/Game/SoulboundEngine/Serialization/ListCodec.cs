namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common.Patterns;
	using System.Collections.Generic;

	public record ListCodec<T>(Codec<T> elementCodec) : Codec<List<T>> {
		public override JToken Encode(List<T> value) {
			JArray array = new();
			foreach (T element in value) {
				array.Add(this.elementCodec.Encode(element));
			}
			return array;
		}

		public override DataResult<List<T>> Decode(JToken json) {
			if (json is not JArray array) {
				return DataResult<List<T>>.Error($"Expected array, got {json.Type}");
			}

			List<T> results = new(array.Count);
			List<string> errors = new();

			for (int i = 0; i < array.Count; i++) {
				DataResult<T> elementResult = this.elementCodec.Decode(array[i]);
				Optional<T> value = elementResult.ResultOrPartial(msg => errors.Add($"[{i}]: msg"));
				if (value.IsPresent()) {
					results.Add(value.GetValue());
				}
			}

			if (errors.Count > 0) {
				string message = string.Join("; ", errors);
				return DataResult<List<T>>.Error(message, results);
			}

			return DataResult<List<T>>.Success(results);
		}
	}
}
