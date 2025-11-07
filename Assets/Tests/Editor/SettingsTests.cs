using NUnit.Framework;
using SoulboundBackend.Client.Settings;
using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

public class SettingsTests {
	private enum TestEnum {
		Value1, Value2, Value3
	}

	private string tempDir;
	private string tempPath;

	[SetUp]
	public void Setup() {
		tempDir = Path.Combine(Path.GetTempPath(), "SoulboundTests");
		Directory.CreateDirectory(tempDir);
		tempPath = Path.Combine(tempDir, "settings.txt");
		File.Delete(tempPath);
	}

	[TearDown]
	public void Cleanup() {
		if (Directory.Exists(tempDir))
			Directory.Delete(tempDir, true);
	}

	[Test]
	public void IntRange_ValidatesRangeCorrectly() {
		var range = new IntRange(0, 10);

		Assert.That(range.IsValid(0));
		Assert.That(range.IsValid(10));
		Assert.That(!range.IsValid(-1));
		Assert.That(!range.IsValid(11));
	}

	[Test]
	public void EnumValueSet_OnlyAcceptsDeclaredValues() {
		var range = new EnumValueSet<TestEnum>(new[] { TestEnum.Value1, TestEnum.Value2 });

		Assert.That(range.IsValid(TestEnum.Value1));
		Assert.That(!range.IsValid(TestEnum.Value3));

		var encoded = range.Encode(TestEnum.Value1);
		var decoded = range.Decode(encoded);
		Assert.That(decoded, Is.EqualTo(TestEnum.Value1));
	}

	[Test]
	public void SettingEntry_RejectsInvalidValues() {
		var entry = new SettingEntry<int>("testEntry", 50, new IntRange(0, 100), null);

		LogAssert.Expect(LogType.Warning, "[SettingEntry]: Attempted to set invalid value 120 to setting testEntry");
		entry.SetValue(120);

		Assert.That(entry.boxedValue, Is.EqualTo(50));
	}

	[Test]
	public void SettingEntry_TriggersValueChangedEvent() {
		var entry = new SettingEntry<int>("test", 50, new IntRange(0, 100), null);
		bool triggered = false;

		entry.valueChanged += (oldVal, newVal) => {
			triggered = true;
			Assert.That(oldVal, Is.EqualTo(50));
			Assert.That(newVal, Is.EqualTo(75));
		};

		entry.SetValue(75);
		Assert.That(triggered, Is.True);
		Assert.That(entry.value, Is.EqualTo(75));
	}

	[Test]
	public void SettingWriter_WritesAndReader_ReadsCorrectly() {
		var entry = new SettingEntry<int>("testEntry", 80, new IntRange(0, 100), null);

		using (var writer = new StreamWriter(tempPath)) {
			var processor = new SettingWriter(writer);
			processor.Process(entry);
		}

		Assert.That(File.Exists(tempPath));

		using (var reader = new StreamReader(tempPath)) {
			var settingReader = new SettingReader(reader);
			int newValue = settingReader.Process(entry);
			Assert.That(newValue, Is.EqualTo(80));
		}
	}

	[Test]
	public void FileValue_GetsOverriden_WhenChangingValue() {
		var entry = new SettingEntry<int>("testEntry", 50, new IntRange(0, 100), null);

		void WriteValue() {
			using (var writer = new StreamWriter(tempPath)) {
				var processor = new SettingWriter(writer);
				processor.Process(entry);
			}
		}

		WriteValue();
		Assert.That(File.Exists(tempPath));

		entry.SetValue(51);
		WriteValue();

		using (var reader = new StreamReader(tempPath)) {
			var processor = new SettingReader(reader);
			var value = processor.Process(entry);
			Assert.That(value, Is.EqualTo(51));
		}
	}
}
