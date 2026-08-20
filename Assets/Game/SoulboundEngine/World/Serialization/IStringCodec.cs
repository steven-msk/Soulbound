#nullable enable

namespace SoulboundEngine.Serialization {
	public interface IStringCodec<T> {
		string Encode(T? value);
		T? Decode(string value);
	}
}
