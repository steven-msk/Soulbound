using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Settings {
	[PROTOTYPICAL]
	public abstract class SettingVisual<T> : MonoBehaviour {
		public abstract void Show(SettingEntry<T> settingEntry);
		public abstract void OnValueChanged(T newValue);
	}
}
