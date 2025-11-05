using SoulboundBackend.Client.Settings;
using GlobalSettings = SoulboundBackend.Client.Settings.Settings;
using SoulboundBackend.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public class SettingsMenuScreen : MenuScreen {
		public override void OnShow() {
			base.OnShow();
			var entryGroup = GetComponent<SettingEntryGroup>();
			entryGroup.AddEntry(GlobalSettings.floatSetting);
		}
	}
}
