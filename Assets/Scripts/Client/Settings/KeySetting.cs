using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace SoulboundBackend.Client.SettingSystem {
	[PROTOTYPICAL]
	public class KeySetting : SettingVisual<KeyControl> {
		[SerializeField] private TextMeshProUGUI _text;
		public TextMeshProUGUI text { get => _text; set => _text = value; }

		public override void Build() {
			_text.text = this.settingEntry.value.keyCode.ToString();
		}
	}
}
