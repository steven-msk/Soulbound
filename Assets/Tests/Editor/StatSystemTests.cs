using ModestTree;
using NUnit.Framework;
using SoulboundBackend.Client.Stats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using Assert = NUnit.Framework.Assert;

namespace StatSystemTests {
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
			UnityEngine.Debug.Log("commit");
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
			StatDefinition<TValue> statDefinition, 
			TValue value, 
			bool keepSign, 
			StatApplicationType applicationType = StatApplicationType.Flat) 
			: base(statDefinition, value, keepSign, applicationType) {
		}

		public override void Apply(StatEntry<TValue> entry, ModificationToken modificationToken) {
			UnityEngine.Debug.Log("apply");
			((TestEntry<TValue>)entry).CommitModifier(this, modificationToken);
		}

		public override void Remove(StatEntry<TValue> entry, ModificationToken modificationToken) {
			((TestEntry<TValue>)entry).UncommitModifier(this, modificationToken);
		}
	}

	[TestFixture]
	public class StatSystem {

		private void IgnoreFailingMessages() {
			// Unity version bug, not related to test cases
			LogAssert.ignoreFailingMessages = true;
		}

		[Test]
		public void ApplyModifier_AddsValue() {
			IgnoreFailingMessages();
			var definition = new StatDefinition<int>("def", SupportedApplicationType.Flat, null);
			var entry = new TestEntry<int>(definition, 100);
			var token = new ModificationToken();

			var modifier = new TestValueModifier<int>(definition, 25, true);

			entry.AcceptModifier(modifier, token);

			Assert.AreEqual(125, entry.CalculateValue());
		}

		[Test]
		public void RemoveModifier_RestoresOriginalValue() {
			IgnoreFailingMessages();
			var definition = new StatDefinition<int>("def", SupportedApplicationType.Flat, null);
			var entry = new TestEntry<int>(definition, 100);
			var token = new ModificationToken();

			var modification = new TestValueModifier<int>(definition, 25, true);

			entry.AcceptModifier(modification, token);
			entry.RemoveModifier(modification, token);

			Assert.AreEqual(100, entry.CalculateValue());
		}

		[Test]
		public void MultipleTokensStack() {
			IgnoreFailingMessages();
			var definition = new StatDefinition<int>("def", SupportedApplicationType.Flat, null);
			var entry = new TestEntry<int>(definition, 100);

			var tokenA = new ModificationToken();
			var tokenB = new ModificationToken();

			var mod1 = new TestValueModifier<int>(definition, 10, true);
			var mod2 = new TestValueModifier<int>(definition, 15, true);

			entry.AcceptModifier(mod1, tokenA);
			entry.AcceptModifier(mod2, tokenB);

			Assert.AreEqual(125, entry.CalculateValue());
		}

		[Test]
		public void RemoveModifiers_RemovesOnlyMatchingToken() {
			IgnoreFailingMessages();
			var def = new StatDefinition<int>("Health", SupportedApplicationType.Flat, null);
			var entry = new TestEntry<int>(def, 100);

			var tokenA = new ModificationToken("A");
			var tokenB = new ModificationToken("B");

			var mod1 = new TestValueModifier<int>(def, 10, true);
			var mod2 = new TestValueModifier<int>(def, 15, true);

			entry.AcceptModifier(mod1, tokenA);
			entry.AcceptModifier(mod2, tokenB);

			entry.RemoveModifiers(tokenA);

			Assert.AreEqual(115, entry.CalculateValue());
		}

		[Test]
		public void ModifierDoesNotApplyToWrongType() {
			IgnoreFailingMessages();
			var defInt = new StatDefinition<int>("Health", SupportedApplicationType.Flat, null);
			var defFloat = new StatDefinition<float>("Speed", SupportedApplicationType.Flat, null);

			var entryInt = new TestEntry<int>(defInt, 100);

			var floatMod = new TestValueModifier<float>(defFloat, 5f, true);

			LogAssert.ignoreFailingMessages = false;
			LogAssert.Expect("[IStatEntryModifier`1]: Mistyped entry in Apply, expected System.Single");
			((IStatEntryModifier)floatMod).Apply(entryInt, new ModificationToken());

			Assert.AreEqual(100, entryInt.CalculateValue());
		}

	}
}
