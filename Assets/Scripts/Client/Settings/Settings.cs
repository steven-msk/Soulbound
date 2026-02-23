using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.SettingSystem {
	public sealed class Settings {
		public const string settingsFile = "settings.txt";
		public static readonly Keybinds keybinds = new();
		public static readonly SettingEntry<int> masterVolume = new("Master Volume", "master_volume", 100, new IntRange(0, 100));

		public static readonly SettingEntry<float> floatSetting = new("Float Setting", "float_setting", 10f, new FloatRange(0f, 50f));
		public static readonly SettingEntry<float> floatSetting_2 = new("Float Setting 2", "float_setting_2", 100f, new FloatRange(50f, 1000f));

		public Settings() => LoadEntries();

		private void LoadEntries() {
			try {
				string savePath = GetSavePath();
				FileStream fileStream = File.Open(savePath, FileMode.Open, FileAccess.Read);

				using (StreamReader reader = new(fileStream)) {
					var settingReader = new SettingReader(reader);

					ProcessSettings(settingReader);
					keybinds.ProcessMappings(new KeybindReader(settingReader));
				};
			} catch (FileNotFoundException) {
				Logger.LogWarning("No settings file found. Initiating with default values");
			}		
		}

		public void Save() {
			string savePath = GetSavePath();

			using (StreamWriter writer = new(savePath, append: false)) {
				var settingWriter = new SettingWriter(writer);

				ProcessSettings(settingWriter);
				keybinds.ProcessMappings(new KeybindWriter(settingWriter));
			};
		}

		public void ProcessSettings(ISettingProcessor processor) {
			masterVolume.SetValue(processor.Process(masterVolume));
			floatSetting.SetValue(processor.Process(floatSetting));
			floatSetting_2.SetValue(processor.Process(floatSetting_2));
		}

		public string GetSavePath() {
			string path = Path.Combine(UnityEngine.Application.persistentDataPath, settingsFile);
			return path.Replace('\\', '/');
		}
	}
}
