using SoulboundBackend.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
	public sealed class SettingReader : ISettingProcessor {
		private readonly Dictionary<string, string> mappings = new();

		public SettingReader(StreamReader reader) {
			while (!reader.EndOfStream) {
				string line = reader.ReadLine();
				string trimmed = line.Trim();
				if (string.IsNullOrEmpty(trimmed)) continue;

				string[] parts = trimmed.Split('=');
				if (parts.Length < 2) continue;

				string key = parts[0].Trim();
				string value = parts[1].Trim();
				mappings[key] = value;
			}
		}

		public int Process(SettingEntry<int> entry) {
			Decode(entry, entry.id, out int value);
			return value;
		}

		public double Process(SettingEntry<double> entry) {
			Decode(entry, entry.id, out double value);
			return value;
		}

		public float Process(SettingEntry<float> entry) {
			Decode(entry, entry.id, out float value);
			return value;
		}

		public string Process(SettingEntry<string> entry) {
			Decode(entry, entry.id, out string? value);
			return value;
		}

		public T Process<T>(SettingEntry<T> entry) {
			Decode(entry, entry.id, out T? value);
			return value;
		}

		public void Decode<T>(SettingEntry<T> entry, string id, out T? value) {
			try {
				value = entry.valueSet.Decode(mappings[id]);
			} catch (KeyNotFoundException) {
				value = entry.defaultValue;
			}
		}
	}
}
