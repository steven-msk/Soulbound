using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Core.Serialization {
	public static class BinaryIO {
		public static void WriteArray<T>(this BinaryWriter writer, T[]? array, Action<BinaryWriter, T> write) {
			if (array == null) {
				writer.Write(-1);
				return;
			}

			writer.Write(array.Length);
			foreach (var item in array) {
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
			foreach (var kvp in dictionary) {
				writeKey(writer, kvp.Key);
				writeValue(writer, kvp.Value);
			}
		}

		public static Dictionary<K, V> ReadDictionary<K, V>(
			this BinaryReader reader,
			Func<BinaryReader, K> readKey,
			Func<BinaryReader, V> readValue
		) {
			int count = reader.ReadInt32();
			var dictionary = new Dictionary<K, V>(count);
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
