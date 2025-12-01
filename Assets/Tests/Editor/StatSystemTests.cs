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
using static UnityEditor.Search.SearchColumn;
using static UnityEngine.EventSystems.EventTrigger;
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
			var definition = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new TestEntry<int>(definition, 100);
			var token = new ModificationToken();

			var modifier = new TestValueModifier<int>(25, true);

			entry.AcceptModifier(modifier, token);

			Assert.AreEqual(125, entry.CalculateValue());
		}

		[Test]
		public void RemoveModifier_RestoresOriginalValue() {
			IgnoreFailingMessages();
			var definition = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
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
			var definition = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
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
			var def = new StatDefinition<int>("Health", SupportedApplicationType.FlatOnly);
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
			var defInt = new StatDefinition<int>("Health", SupportedApplicationType.FlatOnly);
			var defFloat = new StatDefinition<float>("Speed", SupportedApplicationType.FlatOnly);

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
			var def = new StatDefinition<int>("def", SupportedApplicationType.PercentageOnly);
			var entry = new StatEntry<int>(def, 10);

			var mod = new ValueModifier<int>(5, true, StatApplicationType.Flat);
			var context = new ValueModificationContext<int>(mod, entry);

			Assert.IsFalse(context.IsValid());

			var def2 = new StatDefinition<float>("def2", SupportedApplicationType.FlatOnly);
			var entry2 = new StatEntry<float>(def2, 0.1f);

			var mod2 = new ValueModifier<float>(0.9f, true, StatApplicationType.Percentage);
			var context2 = new ValueModificationContext<float>(mod2, entry2);

			Assert.IsFalse(context2.IsValid());

			var def3 = new StatDefinition<float>("def3", SupportedApplicationType.FlatAndPercentage);
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
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 1);

			var mod = new ValueModifier<int>(1, true, StatApplicationType.Flat);
			var procedure = new ValueModifier<int>.Add();
			int a = 1;
			int result = procedure.Apply(a, mod, entry);

			Assert.AreEqual(2, result);
		}

		[Test]
		public void ValueModifier_Subtract_SubtractsValuesCorrectly() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 1);

			var mod = new ValueModifier<int>(75, true, StatApplicationType.Flat);
			var procedure = new ValueModifier<int>.Subtract();
			int a = 100;
			int result = procedure.Apply(a, mod, entry);

			Assert.AreEqual(25, result);
		}

		[Test]
		public void ValueModifier_Multiply_MultipliesValuesCorrectly() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<float>(def, 1f);

			var mod = new ValueModifier<float>(2f, true, StatApplicationType.Flat);
			var procedure = new ValueModifier<float>.Multiply();
			float a = 1.5f;
			float result = procedure.Apply(a, mod, entry);

			Assert.AreEqual(3f, result, 0.0001f);
		}

		[Test]
		public void ValueModifier_Divide_DividesValuesCorrectly() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<float>(def, 1f);

			var mod = new ValueModifier<float>(2f, true, StatApplicationType.Flat);
			var procedure = new ValueModifier<float>.Divide();
			float a = 3f;
			float result = procedure.Apply(a, mod, entry);

			Assert.AreEqual(1.5f, result, 0.0001f);
		}
	}

	[TestFixture]
	public class ProcessorTests {
		[Test]
		public void Apply_AddModifier_UpdatesFinalValue() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 10);
			var mod = new ValueModifier<int>(5, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<int>.Add());

			int finalValue = entry.GetProcessedValue();

			Assert.AreEqual(15, finalValue);
		}

		[Test]
		public void Apply_SubtractModifier_UpdatesFinalValue() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 10);
			var mod = new ValueModifier<int>(3, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<int>.Subtract());

			int finalValue = entry.GetProcessedValue();

			Assert.AreEqual(7, finalValue);
		}

		[Test]
		public void Apply_MultiplyModifier_UpdatesFinalValue() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 10);
			var mod = new ValueModifier<int>(2, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<int>.Multiply());

			int finalValue = entry.GetProcessedValue();

			Assert.AreEqual(20, finalValue);
		}

		[Test]
		public void Apply_DivideModifier_UpdatesFinalValue() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 10);
			var mod = new ValueModifier<int>(2, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<int>.Divide());

			int finalValue = entry.GetProcessedValue();

			Assert.AreEqual(5, finalValue);
		}

		[Test]
		public void Apply_MultipleModifiers_CalculatesCorrectly() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 10);
			var token = new ModificationToken();

			entry.CommitModifier(new ValueModifier<int>(5, true), token, new ValueModifier<int>.Add());		  // 10 + 5 = 15
			entry.CommitModifier(new ValueModifier<int>(10, true), token, new ValueModifier<int>.Subtract()); // 15 - 10 = 5 
			entry.CommitModifier(new ValueModifier<int>(6, true), token, new ValueModifier<int>.Multiply());  // 5 * 6 = 30
			entry.CommitModifier(new ValueModifier<int>(2, true), token, new ValueModifier<int>.Divide());	  // 30 / 2 = 15

			int finalValue = entry.GetProcessedValue();

			Assert.AreEqual(15, finalValue);
		}

		[Test]
		public void Remove_RemovesModificationValue() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 10);
			var token = new ModificationToken();

			var mod = new ValueModifier<int>(5, true);
			entry.CommitModifier(mod, token, new ValueModifier<int>.Add());
			Assert.AreEqual(15, entry.GetProcessedValue());

			entry.UncommitModifier(mod, token);
			Assert.AreEqual(10, entry.GetProcessedValue());
		}

		[Test]
		public void Apply_AddModifier_UpdatesFinalValue_Float() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<float>(def, 0f);
			var mod = new ValueModifier<float>(10.3f, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<float>.Add());

			float finalValue = entry.GetProcessedValue();

			Assert.AreEqual(10.3f, finalValue);
		}

		[Test]
		public void Apply_SubtractModifier_UpdatesFinalValue_Float() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<float>(def, 100f);
			var mod = new ValueModifier<float>(49.9f, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<float>.Subtract());

			float finalValue = entry.GetProcessedValue();

			Assert.AreEqual(50.1f, finalValue, 0.0001f);
		}

		[Test]
		public void Apply_MultiplyModifier_UpdatesFinalValue_Float() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<float>(def, 1.5f);
			var mod = new ValueModifier<float>(4f, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<float>.Multiply());

			float finalValue = entry.GetProcessedValue();

			Assert.AreEqual(6f, finalValue, 0.0001f);
		}

		[Test]
		public void Apply_DivideModifier_UpdatesFinalValue_Float() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<float>(def, 5f);
			var mod = new ValueModifier<float>(2f, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<float>.Divide());

			float finalValue = entry.GetProcessedValue();

			Assert.AreEqual(2.5f, finalValue, 0.0001f);
		}

		[Test]
		public void Apply_SubtractModifier_NegativeFloat() {
			var def = new StatDefinition<float>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<float>(def, 10f);
			var mod = new ValueModifier<float>(-3.5f, true);
			var token = new ModificationToken();
			entry.CommitModifier(mod, token, new ValueModifier<float>.Subtract());
			Assert.AreEqual(13.5f, entry.GetProcessedValue(), 0.0001f); // 10 - (-3.5) = 13.5
		}

		[Test]
		public void Apply_ModifiersWithZero_Int() {
			var def = new StatDefinition<int>("def", SupportedApplicationType.FlatOnly);
			var entry = new StatEntry<int>(def, 10);
			var token = new ModificationToken();

			var addZero = new ValueModifier<int>(0, true);
			entry.CommitModifier(addZero, token, new ValueModifier<int>.Add());
			Assert.AreEqual(10, entry.GetProcessedValue());

			var multZero = new ValueModifier<int>(0, false);
			entry.CommitModifier(multZero, token, new ValueModifier<int>.Multiply());
			Assert.AreEqual(0, entry.GetProcessedValue());
		}
	}
}
