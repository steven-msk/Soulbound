using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.Settings {
	public class SliderSetting : SettingVisual<float> {
		[SerializeField] private Slider _slider;
		public Slider slider { get => _slider; set => _slider = value; }
		private SettingEntry<float> settingEntry;
		private ValueSet<float> valueSet;

		public override void Bind(SettingEntry<float> settingEntry, ValueSet<float> valueSet) {
			this.settingEntry = settingEntry;
			this.valueSet = valueSet;
			_slider.onValueChanged.AddListener(OnValueChanged);
		}

		public override void OnValueChanged(float newValue) {
			settingEntry.SetValue(newValue);
		}
	}
}
