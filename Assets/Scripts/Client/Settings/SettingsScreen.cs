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
	public class SettingsScreen : Screen {
		private SettingEntryGroup entryGroup;

		public override void OnShow() {
			base.OnShow();
			entryGroup = GetComponentInChildren<SettingEntryGroup>();
			entryGroup.AddEntry(GlobalSettings.floatSetting);
			entryGroup.AddEntry((AbstractSettingEntry)GlobalSettings.floatSetting_2);
		}

		//public override void OnHide() {
		//	base.OnHide();
		//	entryGroup.DestroyVisuals();
		//}
	}
}
