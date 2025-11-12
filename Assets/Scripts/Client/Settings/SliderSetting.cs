using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.Settings {
	[PROTOTYPICAL]
	public class SliderSetting : SettingVisual<float> {
		[SerializeField] private Slider _slider;
		public Slider slider { get => _slider; set => _slider = value; }
		private SettingEntry<float> settingEntry;

		public override void Show(SettingEntry<float> settingEntry) {
			this.settingEntry = settingEntry;

			_slider.value = settingEntry.value;
			_slider.onValueChanged.AddListener(OnValueChanged);
		}

		public override void OnValueChanged(float newValue) {
			settingEntry.SetValue(newValue);
		}
	}
}
