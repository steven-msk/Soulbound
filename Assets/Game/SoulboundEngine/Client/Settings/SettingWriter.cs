using System;
using System.IO;

namespace SoulboundEngine.Client.Settings {
	public sealed class SettingWriter : ISettingProcessor {
		private readonly TextWriter writer;

		public SettingWriter(TextWriter writer) {
			this.writer = writer;
		}

		public T Process<T>(SettingEntry<T> entry) {
			this.writer.WriteLine(this.Format(entry.id, entry.value, entry.valueSet.Encode));
			return entry.value;
		}

		public void Flush() => this.writer.Flush();

		public double ProcessDouble(string key, double current) {
			this.writer.WriteLine(this.Format(key, current));
			return current;
		}

		public bool ProcessBoolean(string key, bool current) {
			this.writer.WriteLine(this.Format(key, current));
			return current;
		}

		public float ProcessFloat(string key, float current) {
			this.writer.WriteLine(this.Format(key, current));
			return current;
		}

		public int ProcessInt(string key, int current) {
			this.writer.WriteLine(this.Format(key, current));
			return current;
		}

		public string ProcessString(string key, string current) {
			this.writer.WriteLine(this.Format(key, current));
			return current;
		}

		public T ProcessObject<T>(string key, T current, Func<string, T> decoder, Func<T, string> encoder) {
			this.writer.WriteLine(this.Format(key, current, encoder));
			return current;
		}

		private string Format<T>(string key, T value) => this.Format(key, value, v => v.ToString());

		public string Format<T>(string key, T value, Func<T, string> encoder) {
			return $"{key}={encoder(value)}";
		}

	}
}
