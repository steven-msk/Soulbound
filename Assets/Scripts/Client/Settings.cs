using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Settings {
	public sealed class Settings {
		private static readonly Logger logger = Logger.CreateInstance();
		public const string settingsFile = "settings.txt";
		public static readonly SettingEntry<int> masterVolume = new("master_volume", 100, new IntRange(0, 100));

		public static readonly SettingEntry<float> floatSetting = new("float_setting", 10f, new FloatRange(0f, 50f));
		public static readonly SettingEntry<float> floatSetting_2 = new("float_setting_2", 100f, new FloatRange(50f, 1000f));

		public Settings() => LoadEntries();

		private void LoadEntries() {
			try {
				string savePath = GetSavePath();
				FileStream fileStream = File.Open(savePath, FileMode.Open, FileAccess.Read);

				using (StreamReader reader = new StreamReader(fileStream)) {
					ProcessSettings(new SettingReader(reader));
				};
			} catch (FileNotFoundException) {
				logger.LogWarning(null, "No settings file found. Initiating with default values");
			}			
		}

		public void Save() {
			string savePath = GetSavePath();
			FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.Write);

			using (StreamWriter writer = new StreamWriter(fileStream)) {
				ProcessSettings(new SettingWriter(writer));
			};
		}

		public void ProcessSettings(ISettingProcessor processor) {
			masterVolume.SetValue(processor.Process(masterVolume));
		}

		public string GetSavePath() {
			string path = Path.Combine(UnityEngine.Application.persistentDataPath, settingsFile);
			return path.Replace('\\', '/');
		}
	}
}
