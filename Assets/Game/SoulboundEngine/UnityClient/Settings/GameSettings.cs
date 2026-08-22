using SoulboundEngine.UnityClient.Debug.Logging;
using System.IO;

namespace SoulboundEngine.UnityClient.Settings {
	public sealed class GameSettings {
		public const string settingsFile = "settings.txt";
		public static readonly Keybinds keybinds = new();
		public static readonly SettingEntry<int> masterVolume = new("master_volume", 100, new IntRange(0, 100));

		public static readonly SettingEntry<float> floatSetting = new("float_setting", 10f, new FloatRange(0f, 50f));
		public static readonly SettingEntry<float> floatSetting_2 = new("float_setting_2", 100f, new FloatRange(50f, 1000f));

		public GameSettings() => this.LoadEntries();

		private void LoadEntries() {
			try {
				string savePath = this.GetSavePath();
				FileStream fileStream = File.Open(savePath, FileMode.Open, FileAccess.Read);

				using (StreamReader reader = new(fileStream)) {
					SettingReader settingReader = new(reader);

					this.ProcessSettings(settingReader);
					keybinds.Process(settingReader);
				};
			} catch (FileNotFoundException) {
				Logger.LogWarning("No settings file found. Initiating with default values");
			}		
		}

		public void Save() {
			string savePath = this.GetSavePath();

			using (StreamWriter writer = new(savePath, append: false)) {
				SettingWriter settingWriter = new(writer);

				this.ProcessSettings(settingWriter);
				keybinds.Process(settingWriter);
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
