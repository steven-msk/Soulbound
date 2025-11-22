using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundBackend.Client.SettingSystem {
	[PROTOTYPICAL]
	public class SliderSetting : SettingVisual<float> {
		[SerializeField] private Slider _slider;
		public Slider slider { get => _slider; set => _slider = value; }

		public override void Build() {
			_slider.value = settingEntry.value;
			_slider.onValueChanged.AddListener(OnValueChanged);
		}
	}
}
