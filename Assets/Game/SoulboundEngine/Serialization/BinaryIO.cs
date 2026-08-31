namespace SoulboundEngine.Serialization {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

#nullable enable

	public static class BinaryIO {
		public static void WriteArray<T>(this BinaryWriter writer, T[]? array, Action<BinaryWriter, T> write) {
			if (array == null) {
				writer.Write(-1);
				return;
			}

			writer.Write(array.Length);
			foreach (T? item in array) {
				write(writer, item);
			}
		}

		public static T[]? ReadArray<T>(this BinaryReader reader, Func<BinaryReader, T> read) {
			int length = reader.ReadInt32();
			if (length < 0) {
				return null;
			}

			T[] arr = new T[length];
			for (int i = 0; i < length; i++) {
				arr[i] = read(reader);
			}

			return arr;
		}

		public static void WriteDictionary<K, V>(
			this BinaryWriter writer,
			Dictionary<K, V> dictionary,
			Action<BinaryWriter, K> writeKey,
			Action<BinaryWriter, V> writeValue
		) {
			writer.Write(dictionary.Count);
			foreach ((K key, V value) in dictionary) {
				writeKey(writer, key);
				writeValue(writer, value);
			}
		}

		public static Dictionary<K, V> ReadDictionary<K, V>(
			this BinaryReader reader,
			Func<BinaryReader, K> readKey,
			Func<BinaryReader, V> readValue
		) {
			int count = reader.ReadInt32();
			Dictionary<K, V> dictionary = new(count);
			for (int i = 0; i < count; i++) {
				K key = readKey(reader);
				V value = readValue(reader);
				dictionary[key] = value;
			}
			return dictionary;
		}

		public static void WriteList<T>(this BinaryWriter writer, List<T> list, Action<BinaryWriter, T> write) {
			writer.WriteArray(list.ToArray(), write);
		}

		public static List<T> ReadList<T>(this BinaryReader reader, List<T> list, Func<BinaryReader, T> read) {
			return reader.ReadArray(read).ToList();
		}
	}
}
