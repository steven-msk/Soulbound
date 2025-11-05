using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Settings {
	public abstract class SettingVisual<T> : MonoBehaviour {
		public abstract void Bind(SettingEntry<T> settingEntry, ValueSet<T> valueSet);
		public abstract void OnValueChanged(T newValue);
	}
}
