namespace SoulboundBackend.Core {
	public interface ISerializable<T> {
		public T Serialize();

		public void Deserialize(T serialized);
	}

	public interface ISerializable<TSerialized, TResult> {
		public TSerialized Serialize();

		public TResult Deserialize(TSerialized serialized);
	}
}
