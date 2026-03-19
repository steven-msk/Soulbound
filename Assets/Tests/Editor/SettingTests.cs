using NUnit.Framework;
using SoulboundBackend.Client.SettingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.TestTools;

namespace SettingTests {
	[TestFixture]
	public class EntryTests {
		private enum TestEnum {
			Value1, Value2, Value3
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
		public void FloatRange_ValidatesRangeCorrectly() {
			var range = new FloatRange(0f, 10f);

			Assert.That(range.IsValid(0f));
			Assert.That(range.IsValid(10f));
			Assert.That(!range.IsValid(-0.1f));
			Assert.That(!range.IsValid(11.1f));
		}

		[Test]
		public void DoubleRange_ValidatesRangeCorrectly() {
			var range = new DoubleRange(0d, 10d);

			Assert.That(range.IsValid(0d));
			Assert.That(range.IsValid(10d));
			Assert.That(!range.IsValid(-0.000001d));
			Assert.That(!range.IsValid(10.1101d));
		}

		[Test]
		public void IntRange_EncodesAndDecodesCorrectly() {
			var range = new IntRange(0, 100);
			int value = 70;

			string encoded = range.Encode(value);
			int decoded = range.Decode(encoded);

			Assert.That(decoded, Is.EqualTo(value));
		}

		[Test]
		public void FloatRange_EncodesAndDecodesCorrectly() {
			var range = new FloatRange(0f, 10f);
			float value = 7.5f;

			string encoded = range.Encode(value);
			float decoded = range.Decode(encoded);

			Assert.That(decoded, Is.EqualTo(value));
		}

		[Test]
		public void DoubleRange_EncodesAndDecodesCorrectly() {
			var range = new DoubleRange(-10d, 10d);
			double value = -5d;

			string encoded = range.Encode(value);
			double decoded = range.Decode(encoded);

			Assert.That(decoded, Is.EqualTo(value));
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
			var entry = new SettingEntry<int>("", "testEntry", 50, new IntRange(0, 100), null);

			entry.SetValue(120);

			Assert.That(entry.boxedValue, Is.EqualTo(50));
		}

		[Test]
		public void SettingEntry_RejectsIdenticValues() {
			var entry = new SettingEntry<float>("", "testEntry", 0.75f, new FloatRange(0f, 10f), null);

			bool valueChanged = false;
			entry.valueChanged += (_, _) => valueChanged = true;

			entry.SetValue(0.75f);

			Assert.That(!valueChanged);
			Assert.That(entry.value, Is.EqualTo(0.75f));
		}

		[Test]
		public void SettingEntry_RejectsNullValues() {
			var entry = new SettingEntry<string>("", "testEntry", "default", new StringValueSet(new[] { "default", "option1", "option2" }), null);

			bool valueChanged = false;
			entry.valueChanged += (_, _) => valueChanged = true;

			entry.SetValue(null!);

			Assert.That(!valueChanged);
			Assert.That(entry.value, Is.EqualTo("default"));
		}

		[Test]
		public void SettingEntry_TriggersValueChangedEvent() {
			var entry = new SettingEntry<int>("", "test", 50, new IntRange(0, 100), null);
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
	}

	[TestFixture]
	public class SerializationTests {
		private string tempDir;
		private string tempPath;

		[SetUp]
		public void Setup() {
			tempDir = Path.Combine(Path.GetTempPath(), "SoulboundTests", Guid.NewGuid().ToString());
			Directory.CreateDirectory(tempDir);
			tempPath = Path.Combine(tempDir, "settings.txt");
			File.Create(tempPath).Close();
		}

		[TearDown]
		public void Cleanup() {
			if (Directory.Exists(tempDir)) {
				Directory.Delete(tempDir, true);
			}
		}

		[Test]
		public void SettingWriter_WritesAndReader_ReadsCorrectly() {
			const int value = 80;

			var entry = new SettingEntry<int>("", "testEntry", value, new IntRange(0, 100), null);

			using (var writer = new StreamWriter(tempPath)) {
				var processor = new SettingWriter(writer);
				processor.Process(entry);
			}

			Assert.That(File.Exists(tempPath));

			using (var reader = new StreamReader(tempPath)) {
				var settingReader = new SettingReader(reader);
				int newValue = settingReader.Process(entry);
				Assert.That(newValue, Is.EqualTo(value));
			}
		}

		[Test]
		public void SettingWriter_ReadsBackIntCorrectly() {
			const int value = 42;

			var entry = new SettingEntry<int>("", "testInt", value, new IntRange(0, 100), null);

			using (var writer = new StreamWriter(tempPath)) {
				new SettingWriter(writer).Process(entry);
			}

			using (var reader = new StreamReader(tempPath)) {
				int result = new SettingReader(reader).Process(entry);

				Assert.That(result, Is.EqualTo(value));
			}
		}

		[Test]
		public void SettingWriter_ReadsBackFloatCorrectly() {
			const float value = 3.14f;

			var entry = new SettingEntry<float>("", "testFloat", value, new FloatRange(0f, 5f), null);

			using (var writer = new StreamWriter(tempPath)) {
				new SettingWriter(writer).Process(entry);
			}

			using (var reader = new StreamReader(tempPath)) {
				var result = new SettingReader(reader).Process(entry);

				Assert.That(result, Is.EqualTo(value));
			}
		}

		[Test]
		public void SettingWriter_ReadsBackDoubleCorrectly() {
			const double value = 0.476846178;

			var entry = new SettingEntry<double>("", "testDouble", value, new DoubleRange(0d, 1d), null);

			using (var writer = new StreamWriter(tempPath)) {
				new SettingWriter(writer).Process(entry);
			}

			using (var reader = new StreamReader(tempPath)) {
				var result = new SettingReader(reader).Process(entry);

				Assert.That(result, Is.EqualTo(value));
			}
		}

		[Test]
		public void SettingWriter_ReadsBackStringCorrectly() {
			const string value = "someString";

			var entry = new SettingEntry<string>("", "testString", value, new StringValueSet(new[] { value }), null);

			using (var writer = new StreamWriter(tempPath)) {
				new SettingWriter(writer).Process(entry);
			}

			using (var reader = new StreamReader(tempPath)) {
				var result = new SettingReader(reader).Process(entry);

				Assert.That(result, Is.EqualTo(value));
			}
		}

		[Test]
		public void SettingWriter_ReadsKeyControlStringCorrectly() {
			var entry = new KeybindEntry("", "keyTest", Key.Space, null);

			using (var writer = new StreamWriter(tempPath)) {
				new SettingWriter(writer).Process(entry);
			}

			using (var reader = new StreamReader(tempPath)) {
				var result = new SettingReader(reader).Process(entry);

				Assert.That(result.keyCode, Is.EqualTo(Key.Space));
			}
		}

		[Test]
		public void SettingWriter_ReadsBackMultipleEntriesCorrectly() {
			string[] strings = { "o1", "o2", "03" };
			const int intValue = 10;

			var intEntry = new SettingEntry<int>("Int", "intSetting", intValue, new IntRange(0, 100), null);
			var strEntry = new SettingEntry<string>("Str", "stringSetting", strings[0], new StringValueSet(strings), null);

			using (var writer = new StreamWriter(tempPath)) {
				var settingWriter = new SettingWriter(writer);
				settingWriter.Process(intEntry);
				settingWriter.Process(strEntry);
			}

			using (var reader = new StreamReader(tempPath)) {
				var settingReader = new SettingReader(reader);
				Assert.That(settingReader.Process(intEntry), Is.EqualTo(intValue));
				Assert.That(settingReader.Process(strEntry), Is.EqualTo(strings[0]));
			}
		}

		[Test]
		public void SettingReader_UsesDefaultValue_WhenEntryIsMissing() {
			const int defaultValue = 50;

			var entry = new SettingEntry<int>("", "missingInt", defaultValue, new IntRange(0, 100), null);

			using (var writer = new StreamWriter(tempPath)) {
				// file is empty, no writing here
			}

			using (var reader = new StreamReader(tempPath)) {
				int result = new SettingReader(reader).Process(entry);

				Assert.That(result, Is.EqualTo(defaultValue));
			}
		}

		[Test]
		public void SettingWriter_WritesCorrectFormat() {
			const string key = "fmtsi.int";
			var entry = new SettingEntry<int>("", key, 7, new IntRange(0, 10), null);

			using (var writer = new StreamWriter(tempPath)) {
				new SettingWriter(writer).Process(entry);
			}

			string line = File.ReadAllText(tempPath).Trim();

			Assert.That(line, Is.EqualTo($"{key}={7}"));
		}

		[Test]
		public void SettingReader_IgnoresEmptyLinesAndWhiteSpace() {
			const int fileValue = 55;
			const string key = "fmtsi.value";
			File.WriteAllText(tempPath, $"\n\n       {key}={fileValue}    \n \n");
			var entry = new SettingEntry<int>("", key, 0, new IntRange(0, 10), null);

			using (var reader = new StreamReader(tempPath)) {
				int result = new SettingReader(reader).Process(entry);

				Assert.That(result, Is.EqualTo(fileValue));
			}
		}

		[Test]
		public void FileValue_GetsOverriden_WhenChangingValue() {
			var entry = new SettingEntry<int>("", "testEntry", 50, new IntRange(0, 100), null);

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

	[TestFixture]
	public class KeyMappingTests {
		[Test]
		public void KeyboardValueSet_Encode_ReturnsKeyEnumString() {
			var valueSet = new KeyboardValueSet();
			var keyControl = Keyboard.current[Key.G];

			string encoded = valueSet.Encode(keyControl);

			Assert.That(encoded, Is.EqualTo("G"));
		}

		[Test]
		public void KeyboardValueSet_Encode_ReturnsNull_ForNull() {
			var valueSet = new KeyboardValueSet();

			string encoded = valueSet.Encode(null);

			Assert.That(encoded, Is.EqualTo("null"));
		}

		[Test]
		public void KeyboardValueSet_Decode_ParsesKeyEnum() {
			var valueSet = new KeyboardValueSet();

			KeyControl decoded = valueSet.Decode("H");

			Assert.That(decoded, Is.EqualTo(Keyboard.current[Key.H]));
		}

		[Test]
		public void KeyboardValueSet_Decode_ReturnsNull_ForInvalidKey() {
			var valueSet = new KeyboardValueSet();

			KeyControl decoded = valueSet.Decode("Invalid Key");

			Assert.That(decoded, Is.Null);
		}

		[Test]
		public void KeyboardValueSet_EncodeAndDecode_ReturnCorrectValues() {
			var valueSet = new KeyboardValueSet();
			var key = Keyboard.current[Key.A];

			string encoded = valueSet.Encode(key);
			KeyControl decoded = valueSet.Decode(encoded);

			Assert.That(key, Is.EqualTo(decoded));
		}

		[Test]
		public void KeyboardValueSet_IsValid_ReturnsTrueForKeyboardKey() {
			var valueSet = new KeyboardValueSet();
			var keyControl = Keyboard.current[Key.Space];

			Assert.That(valueSet.IsValid(keyControl), Is.True);
		}

		[Test]
		public void KeyboardValueSet_IsValid_ReturnsTrueForNull() {
			var valueSet = new KeyboardValueSet();

			Assert.That(valueSet.IsValid(null), Is.True);
		}

		[Test]
		public void KeyMapping_SetKey_ChangesKeyControl() {
			var keyMapping = new KeybindEntry("Test", "test", Key.N, null);

			keyMapping.SetValue(Key.T);

			Assert.That(keyMapping.value, Is.EqualTo(Keyboard.current[Key.T]));
		}

		[Test]
		public void KeyMappingWriter_WritesCorrectFormat() {
			var stringWriter = new StringWriter();
			var writer = new SettingWriter(stringWriter);
			var keyMapping = new KeybindEntry("Test", "test", Key.L, null);

			keyMapping.SetValue(Key.V);

			var processor = new KeybindWriter(writer);
			processor.Process(keyMapping);
			writer.Flush();

			string expectedLine = $"keybind.test=V";
			StringAssert.Contains(expectedLine, stringWriter.ToString());
		}

		[Test]
		public void KeyMappingReader_ReadsStoredKeyCorrectly() {
			string line = "keybind.test=H\n";
			var settingReader = new SettingReader(
				new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(line)))
			);
			var keyMapping = new KeybindEntry("Test", "test", Key.G, null);

			var reader = new KeybindReader(settingReader);

			KeyControl result = reader.Process(keyMapping);

			Assert.That(result, Is.EqualTo(Keyboard.current[Key.H]));
		}
	}
}
