namespace SoulboundBackend.Core {
	public interface ISerializable<T> {
		public T Serialize();

		public void Deserialize(T serialized);
	}
}
