using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.Stats {
	public interface IStatEntry {
		object CalculateBoxedValue();
		Type valueType { get; }

		void AcceptModifier(IStatEntryModifier modifier, ModificationToken modificationToken);
		void RemoveModifier(IStatEntryModifier modifier, ModificationToken modificationToken);
		void RemoveModifiers(ModificationToken modificationToken);
	}
}