using SoulboundBackend.Client.UI.Tooltip;
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
		[SerializeField] private TooltipTrigger _tooltipTrigger;
		public TooltipTrigger tooltipTrigger { 
			get => _tooltipTrigger; 
			set => SetTooltipTrigger(value);
		}
		private SettingEntry<float> settingEntry;

		public override void Show(SettingEntry<float> settingEntry) {
			this.settingEntry = settingEntry;

			_slider.value = settingEntry.value;
			_slider.onValueChanged.AddListener(OnValueChanged);
			_tooltipTrigger.Init(settingEntry.tooltipSupplier());
		}

		protected override void SetTooltipTrigger(TooltipTrigger trigger) {
			this._tooltipTrigger = trigger;
		}

		public override void OnValueChanged(float newValue) {
			settingEntry.SetValue(newValue);
		}
	}
}
