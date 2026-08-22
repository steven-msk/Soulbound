using SoulboundEngine.UnityClient.Debug.Logging;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable

namespace SoulboundEngine.UnityClient.Settings {
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
				this.mappings[key] = value;
			}
		}

		public T Process<T>(SettingEntry<T> entry) {
			this.Decode(entry, entry.id, out T value);
			return value;
		}

		public void Decode<T>(SettingEntry<T> entry, string id, out T value) {
			try {
				value = entry.valueSet.Decode(this.mappings[id]);
			} catch (KeyNotFoundException) {
				value = entry.defaultValue;
			}
		}

		public double ProcessDouble(string key, double current) {
			return this.ProcessObject(key, current, double.Parse, d => d.ToString());
		}

		public bool ProcessBoolean(string key, bool current) {
			return this.ProcessObject(key, current, bool.Parse, b => b.ToString());
		}

		public float ProcessFloat(string key, float current) {
			return this.ProcessObject(key, current, float.Parse, f => f.ToString());
		}

		public int ProcessInt(string key, int current) {
			return this.ProcessObject(key, current, int.Parse, f => f.ToString());
		}

		public string ProcessString(string key, string current) {
			return this.ProcessObject(key, current, s => s, s => s);
		}

		public T ProcessObject<T>(string key, T current, Func<string, T> decoder, Func<T, string> encoder) {
			if (!this.mappings.TryGetValue(key, out string value)) {
				Logger.LogError("Could not find mapping {}. Reverting to current value", key);
				return current;
			}
			return decoder(value);
		}
	}
}
