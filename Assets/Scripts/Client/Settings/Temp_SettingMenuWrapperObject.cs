using SoulboundBackend.Client.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Temp_SettingMenuWrapperObject : MonoBehaviour {
	private void OnEnable() {
		SettingMenu settingMenu = new TestSettingMenu();
		settingMenu.Init(GetComponent<SettingEntryGroup>());
	}

	private class TestSettingMenu : SettingMenu {
		public override IEnumerable<SettingEntry> GetSettingEntries() {
			yield return Settings.floatSetting;
		}
	}
}
