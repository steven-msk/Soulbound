using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.SettingSystem {
	public sealed class Settings {
		private static readonly Logger logger = Logger.CreateInstance();
		public const string settingsFile = "settings.txt";
		public static readonly KeybindMappings keybindMappings = new();
		public static readonly SettingEntry<int> masterVolume = new("Master Volume", "master_volume", 100, new IntRange(0, 100), Tooltip.NoTooltip);

		public static readonly SettingEntry<float> floatSetting = new("Float Setting", "float_setting", 10f, new FloatRange(0f, 50f), Tooltip.NoTooltip);
		public static readonly SettingEntry<float> floatSetting_2 = new("Float Setting 2", "float_setting_2", 100f, new FloatRange(50f, 1000f), Tooltip.NoTooltip);

		public Settings() => LoadEntries();

		private void LoadEntries() {
			try {
				string savePath = GetSavePath();
				FileStream fileStream = File.Open(savePath, FileMode.Open, FileAccess.Read);

				using (StreamReader reader = new StreamReader(fileStream)) {
					var settingReader = new SettingReader(reader);

					ProcessSettings(settingReader);
					keybindMappings.ProcessMappings(new KeyMappingReader(settingReader));
				};
			} catch (FileNotFoundException) {
				logger.LogWarning(null, "No settings file found. Initiating with default values");
			}		
		}

		public void Save() {
			string savePath = GetSavePath();
			FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.Write);

			using (StreamWriter writer = new StreamWriter(fileStream)) {
				var settingWriter = new SettingWriter(writer);

				ProcessSettings(settingWriter);
				keybindMappings.ProcessMappings(new KeyMappingWriter(settingWriter));
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
