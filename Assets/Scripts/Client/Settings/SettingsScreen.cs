using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SoulboundBackend.Common;

namespace SoulboundBackend.Client.UI.Screens {
	[PROTOTYPICAL]
	public class SettingsScreen : Screen {
		private SettingEntryGroup entryGroup;

		public override void OnShow() {
			base.OnShow();
			entryGroup = GetComponentInChildren<SettingEntryGroup>();
			entryGroup.AddEntry(Settings.floatSetting);
			entryGroup.AddEntry(Settings.floatSetting_2);
			entryGroup.AddEntry(KeybindMappings.jump);
		}

		public override void Dispose() {
			// intended overriding, do not delete
		}

		//public override void OnHide() {
		//	base.OnHide();
		//	entryGroup.DestroyVisuals();
		//}
	}
}
