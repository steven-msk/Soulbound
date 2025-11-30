using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Stats {
	public interface IStatEntry {
		//[Obsolete]
		//void Add(AbstractValueModifier serializableStat, IStatProvider provider);

		//[Obsolete]
		//void AddRange(params (AbstractValueModifier stat, IStatProvider provider)[] modifiers);

		//[Obsolete]
		//void Remove(AbstractValueModifier serializableStat, IStatProvider provider);

		//[Obsolete]
		//void RemoveRange(params (AbstractValueModifier stat, IStatProvider provider)[] modifiers);

		//[Obsolete]
		//void SetModifiers(List<(AbstractValueModifier stat, IStatProvider provider)> modifiers);

		object GetBoxedValue();
		Type valueType { get; }

		//[Obsolete]
		//List<(AbstractValueModifier, IStatProvider)> GetBoxedModifiers();

		[Obsolete]
		internal class UnsupportedSerializableStatTypeException : NullReferenceException {
			public UnsupportedSerializableStatTypeException(object value, Type expectedType)
				: base($"Unsupported stat value type {value.GetType()} for entry of type {expectedType}") {
			}
		}
	}
}