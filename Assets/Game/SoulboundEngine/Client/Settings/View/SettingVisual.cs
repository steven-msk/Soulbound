using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.SettingSystem.View {
	[PROTOTYPICAL]
	public abstract class SettingVisual<T> : MonoBehaviour {
		protected SettingEntry<T> settingEntry;

		protected abstract void Build();

		public virtual void Show(SettingEntry<T> settingEntry) {
			this.settingEntry = settingEntry;
			this.Build();
		}

		public virtual void OnValueChanged(T newValue) {
			settingEntry.SetValue(newValue);
		}
	}
}
