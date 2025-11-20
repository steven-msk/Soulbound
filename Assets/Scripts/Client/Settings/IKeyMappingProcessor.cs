using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem.Controls;

#nullable enable

namespace SoulboundBackend.Client.Settings {
	public interface IKeyMappingProcessor : ISettingProcessor {
		int ISettingProcessor.Process(SettingEntry<int> entry) => throw NotImplemented(typeof(int));
		double ISettingProcessor.Process(SettingEntry<double> entry) => throw NotImplemented(typeof(double));
		float ISettingProcessor.Process(SettingEntry<float> entry) => throw NotImplemented(typeof(float));
		string ISettingProcessor.Process(SettingEntry<string> entry) => throw NotImplemented(typeof(string));
		T ISettingProcessor.Process<T>(SettingEntry<T> entry) => throw NotImplemented(typeof(T));

		KeyControl Process(KeyMapping keyMapping);

		private NotImplementedException NotImplemented(Type type) {
			return new NotImplementedException($"KeyMappingProcessor does not support {type} types");
		}
	}
}
