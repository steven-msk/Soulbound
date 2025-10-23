using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.Settings {
	public sealed class SettingReader : ISettingProcessor {
		private readonly StreamReader reader;

		public SettingReader(StreamReader reader) {
			this.reader = reader;
		}

		public int Process(SettingEntry<int> entry) {
			Decode(entry, reader.ReadLine(), out int value);
			return value;
		}

		public double Process(SettingEntry<double> entry) {
			Decode(entry, reader.ReadLine(), out double value);
			return value;
		}

		public float Process(SettingEntry<float> entry) {
			Decode(entry, reader.ReadLine(), out float value);
			return value;
		}

		public string Process(SettingEntry<string> entry) {
			Decode(entry, reader.ReadLine(), out string? value);
			return value;
		}

		public T Process<T>(SettingEntry<T> entry) {
			Decode(entry, reader.ReadLine(), out T? value);
			return value;
		}

		public void Decode<T>(SettingEntry<T> entry, string line, out T? value) {
			value = entry.valueSet.Decode(line);
		}
	}
}
