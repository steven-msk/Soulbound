using ModestTree;
using NUnit.Framework;
using SoulboundBackend.Client.Stats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using Zenject;
using Assert = NUnit.Framework.Assert;

namespace StatSystemTests {
	[TestFixture]
	public class EntryLogicTests {
		public class TestEntry<TValue> : StatEntry<TValue> where TValue : struct, IComparable<TValue> {
			internal readonly Dictionary<ModificationToken, List<IStatEntryModifier<TValue>>> modifiers = new();

			public TestEntry(StatDefinition<TValue> definition, TValue baseValue)
				: base(definition, baseValue) {
				this.definition = definition;
				this.baseValue = baseValue;
			}

			public Type valueType => typeof(TValue);

			public TValue CalculateValue() {
				dynamic total = baseValue;

				foreach (var list in modifiers.Values) {
					foreach (var modifier in list) {
						total += (dynamic)((ValueModifier<TValue>)modifier).value;
					}
				}
				return total;
			}

			public new void AcceptModifier(IStatEntryModifier modifier, ModificationToken modificationToken) {
				modifier.Apply(this, modificationToken);
			}

			public new object CalculateBoxedValue() => CalculateValue();

			public new void RemoveModifier(IStatEntryModifier modifier, ModificationToken modificationToken) {
				modifier.Remove(this, modificationToken);
			}

			public new void RemoveModifiers(ModificationToken modificationToken) {
				if (modifiers.TryGetValue(modificationToken, out var list)) {
					var toRemove = new List<IStatEntryModifier<TValue>>(list);
					foreach (var modifier in toRemove) {
						modifier.Remove(this, modificationToken);
					}
					list = toRemove;
				}
			}

			public new void CommitModifier(IStatEntryModifier<TValue> modifier, ModificationToken modificationToken) {
				if (!modifiers.TryGetValue(modificationToken, out var list)) {
					modifiers[modificationToken] = list = new();
				}
				list.Add(modifier);
			}

			public new void UncommitModifier(IStatEntryModifier<TValue> modifier, ModificationToken modificationToken) {
				if (modifiers.TryGetValue(modificationToken, out var list)) {
					list.Remove(modifier);
				}
				if (list.IsEmpty()) {
					modifiers.Remove(modificationToken);
				}
			}
		}

		public class TestValueModifier<TValue> : ValueModifier<TValue>
				where TValue : struct, IComparable<TValue> {
			public TestValueModifier(
				TValue value,
				bool keepSign,
				StatApplicationType applicationType = StatApplicationType.Flat)
				: base(value, keepSign, applicationType) {
			}

			public override void Apply(StatEntry<TValue> entry, ModificationToken modificationToken) {
				((TestEntry<TValue>)entry).CommitModifier(this, modificationToken);
			}

			public override void Remove(StatEntry<TValue> entry, ModificationToken modificationToken) {
				((TestEntry<TValue>)entry).UncommitModifier(this, modificationToken);
			}
		}

		private void IgnoreFailingMessages() {
			// Unity version bug, not related to test cases
			LogAssert.ignoreFailingMessages = true;
		}

		[Test]
		public void ApplyModifier_AddsValue() {
			IgnoreFailingMessages();
			var definition = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly, null);
			var entry = new TestEntry<int>(definition, 100);
			var token = new ModificationToken();

			var modifier = new TestValueModifier<int>(25, true);

			entry.AcceptModifier(modifier, token);

			Assert.AreEqual(125, entry.CalculateValue());
		}

		[Test]
		public void RemoveModifier_RestoresOriginalValue() {
			IgnoreFailingMessages();
			var definition = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly, null);
			var entry = new TestEntry<int>(definition, 100);
			var token = new ModificationToken();

			var modification = new TestValueModifier<int>(25, true);

			entry.AcceptModifier(modification, token);
			entry.RemoveModifier(modification, token);

			Assert.AreEqual(100, entry.CalculateValue());
		}

		[Test]
		public void MultipleTokensStack() {
			IgnoreFailingMessages();
			var definition = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly, null);
			var entry = new TestEntry<int>(definition, 100);

			var tokenA = new ModificationToken();
			var tokenB = new ModificationToken();

			var mod1 = new TestValueModifier<int>(10, true);
			var mod2 = new TestValueModifier<int>(15, true);

			entry.AcceptModifier(mod1, tokenA);
			entry.AcceptModifier(mod2, tokenB);

			Assert.AreEqual(125, entry.CalculateValue());
		}

		[Test]
		public void RemoveModifiers_RemovesOnlyMatchingToken() {
			IgnoreFailingMessages();
			var def = new StatDefinition<int>("Health", SupportedApplicationType.FlatOnly, null);
			var entry = new TestEntry<int>(def, 100);

			var tokenA = new ModificationToken("A");
			var tokenB = new ModificationToken("B");

			var mod1 = new TestValueModifier<int>(10, true);
			var mod2 = new TestValueModifier<int>(15, true);

			entry.AcceptModifier(mod1, tokenA);
			entry.AcceptModifier(mod2, tokenB);

			entry.RemoveModifiers(tokenA);

			Assert.AreEqual(115, entry.CalculateValue());
		}

		[Test]
		public void ModifierDoesNotApplyToWrongType() {
			IgnoreFailingMessages();
			var defInt = new StatDefinition<int>("Health", SupportedApplicationType.FlatOnly, null);
			var defFloat = new StatDefinition<float>("Speed", SupportedApplicationType.FlatOnly, null);

			var entryInt = new TestEntry<int>(defInt, 100);

			var floatMod = new TestValueModifier<float>(5f, true);

			LogAssert.ignoreFailingMessages = false;
			LogAssert.Expect("[IStatEntryModifier`1]: Mistyped entry in Apply, expected System.Single");
			((IStatEntryModifier)floatMod).Apply(entryInt, new ModificationToken());

			Assert.AreEqual(100, entryInt.CalculateValue());
		}
	}

	[TestFixture]
	public class ModificationContextTests {
		[Test]
		public void ValueModification_IsValid_ReturnsTrueAccordingly() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.PercentageOnly, null);
			var entry = new StatEntry<int>(def, 10);

			var mod = new ValueModifier<int>(5, true, StatApplicationType.Flat);
			var context = new ValueModificationContext<int>(mod, entry);

			Assert.IsFalse(context.IsValid());

			var def2 = new StatDefinition<float>("def2", SupportedApplicationType.FlatOnly, null);
			var entry2 = new StatEntry<float>(def2, 0.1f);

			var mod2 = new ValueModifier<float>(0.9f, true, StatApplicationType.Percentage);
			var context2 = new ValueModificationContext<float>(mod2, entry2);

			Assert.IsFalse(context2.IsValid());

			var def3 = new StatDefinition<float>("def3", SupportedApplicationType.FlatAndPercentage, null);
			var entry3 = new StatEntry<float>(def3, 0f);

			var mod3 = new ValueModifier<float>(0.1f, true, StatApplicationType.Percentage);
			var mod4 = new ValueModifier<float>(10f, true, StatApplicationType.Flat);
			var context3 = new ValueModificationContext<float>(mod3, entry3);
			var context4 = new ValueModificationContext<float>(mod4, entry3);

			Assert.IsTrue(context3.IsValid());
			Assert.IsTrue(context4.IsValid());
		}
	}

	[TestFixture]
	public class ValueModifierProceduresTests {
		[Test]
		public void ValueModifier_Add_AddsValuesCorrectly() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly, null);
			var entry = new StatEntry<int>(def, 1);

			var procedure = new ValueModifier<int>.Add();
			int a = 1, b = 1;
			int result = procedure.Apply(a, b, entry);

			Assert.AreEqual(2, result);
		}

		[Test]
		public void ValueModifier_Subtract_SubtractsValuesCorrectly() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly, null);
			var entry = new StatEntry<int>(def, 1);

			var procedure = new ValueModifier<int>.Subtract();
			int a = 100, b = 75;
			int result = procedure.Apply(a, b, entry);

			Assert.AreEqual(25, result);
		}

		[Test]
		public void ValueModifier_Multiply_MultipliesValuesCorrectly() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly, null);
			var entry = new StatEntry<float>(def, 1f);

			var procedure = new ValueModifier<float>.Multiply();
			float a = 1.5f, b = 2f;
			float result = procedure.Apply(a, b, entry);

			Assert.AreEqual(3f, result);
		}

		[Test]
		public void ValueModifier_Divide_DividesValuesCorrectly() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly, null);
			var entry = new StatEntry<float>(def, 1f);

			var procedure = new ValueModifier<float>.Divide();
			float a = 3f, b = 2f;
			float result = procedure.Apply(a, b, entry);

			Assert.AreEqual(1.5f, result);
		}
	}
}
