namespace SoulboundEngine.Serialization {
	using System.IO;

	public abstract class BinarySerializer<T> : ISerializer<T> {
		protected abstract T ReadBinary(BinaryReader reader);
		protected abstract byte[] WriteBinary(T obj, BinaryWriter writer);

		public virtual T Deserialize(byte[] data) {
			using MemoryStream memoryStream = new(data);
			using BinaryReader reader = new(memoryStream);

			return this.ReadBinary(reader);
		}

		public virtual byte[] Serialize(T obj) {
			using MemoryStream memoryStream = new();
			using BinaryWriter writer = new(memoryStream);

			this.WriteBinary(obj, writer);

			return memoryStream.ToArray();
		}
	}
}
