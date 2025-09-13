using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Stats {
	public interface IStatEntryImpl {
		void Add(AbstractSerializableStat serializableStat, IStatProvider provider);

		void AddRange(params (AbstractSerializableStat stat, IStatProvider provider)[] modifiers);

		void Remove(AbstractSerializableStat serializableStat, IStatProvider provider);

		void RemoveRange(params (AbstractSerializableStat stat, IStatProvider provider)[] modifiers);

		void SetModifiers(List<(AbstractSerializableStat stat, IStatProvider provider)> modifiers);

		object GetBoxedValue();

		List<(AbstractSerializableStat, IStatProvider)> GetBoxedModifiers();

		internal class UnsupportedSerializableStatTypeException : NullReferenceException {
			public UnsupportedSerializableStatTypeException(object value, Type expectedType)
				: base($"Unsupported stat value type {value.GetType()} for entry of type {expectedType}") {
			}
		}
	}
}