using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Settings {
	public sealed class SettingWriter : ISettingProcessor {
		private readonly StreamWriter writer;

		public SettingWriter(StreamWriter writer) {
			this.writer = writer;
		}

		public int Process(SettingEntry<int> entry) {
			writer.WriteLine(FormatValue(entry));
			return entry.value;
		}

		public double Process(SettingEntry<double> entry) {
			writer.WriteLine(FormatValue(entry));
			return entry.value;
		}

		public float Process(SettingEntry<float> entry) {
			writer.WriteLine(FormatValue(entry));
			return entry.value;
		}

		public string Process(SettingEntry<string> entry) {
			writer.WriteLine(FormatValue(entry));
			return entry.value;
		}

		public T Process<T>(SettingEntry<T> entry) {
			writer.WriteLine(FormatValue(entry));
			return entry.value;
		}

		public string FormatValue<T>(SettingEntry<T> entry) {
			return $"{entry.id}={entry.value}";
		}
	}
}
