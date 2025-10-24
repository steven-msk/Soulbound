using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.Settings {
	public sealed class SettingReader : ISettingProcessor {
		private readonly Dictionary<string, string> mappings = new();

		public SettingReader(StreamReader reader) {
			while (!reader.EndOfStream) {
				string[] parts = reader.ReadLine().Split('=');
				string id = parts[0];
				string value = parts[1];
				mappings[id] = value;
			}
		}

		public int Process(SettingEntry<int> entry) {
			Decode(entry, mappings[entry.id], out int value);
			return value;
		}

		public double Process(SettingEntry<double> entry) {
			Decode(entry, mappings[entry.id], out double value);
			return value;
		}

		public float Process(SettingEntry<float> entry) {
			Decode(entry, mappings[entry.id], out float value);
			return value;
		}

		public string Process(SettingEntry<string> entry) {
			Decode(entry, mappings[entry.id], out string? value);
			return value;
		}

		public T Process<T>(SettingEntry<T> entry) {
			Decode(entry, mappings[entry.id], out T? value);
			return value;
		}

		public void Decode<T>(SettingEntry<T> entry, string stringValue, out T? value) {
			value = entry.valueSet.Decode(stringValue);
		}
	}
}
