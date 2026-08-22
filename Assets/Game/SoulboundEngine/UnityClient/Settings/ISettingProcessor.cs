using System;

#nullable enable

namespace SoulboundEngine.UnityClient.Settings {
	public interface ISettingProcessor {
		T Process<T>(SettingEntry<T> entry);

		double ProcessDouble(string key, double current);
		bool ProcessBoolean(string key, bool current);
		float ProcessFloat(string key, float current);
		int ProcessInt(string key, int current);
		string ProcessString(string key, string current);
		T ProcessObject<T>(string key, T current,  Func<string, T> decoder, Func<T, string> encoder);
	}
}
