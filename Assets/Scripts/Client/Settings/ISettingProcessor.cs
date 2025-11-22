using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.SettingSystem {
	public interface ISettingProcessor {
		int Process(SettingEntry<int> entry);
		double Process(SettingEntry<double> entry);
		float Process(SettingEntry<float> entry);
		string Process(SettingEntry<string> entry);
		T Process<T>(SettingEntry<T> entry);
	}
}
