using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Stats {
	public interface IStatEntry {
		void Add(AbstractValueModifier serializableStat, IStatProvider provider);

		void AddRange(params (AbstractValueModifier stat, IStatProvider provider)[] modifiers);

		void Remove(AbstractValueModifier serializableStat, IStatProvider provider);

		void RemoveRange(params (AbstractValueModifier stat, IStatProvider provider)[] modifiers);

		void SetModifiers(List<(AbstractValueModifier stat, IStatProvider provider)> modifiers);

		object GetBoxedValue();

		List<(AbstractValueModifier, IStatProvider)> GetBoxedModifiers();

		internal class UnsupportedSerializableStatTypeException : NullReferenceException {
			public UnsupportedSerializableStatTypeException(object value, Type expectedType)
				: base($"Unsupported stat value type {value.GetType()} for entry of type {expectedType}") {
			}
		}
	}
}