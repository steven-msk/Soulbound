using SoulboundEngine.Client.ItemSystem;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Stats {
	public interface IStatEntry {
		object CalculateBoxedValue();
		Type valueType { get; }
		IStatDefinition definition { get; }

		void AcceptModifier(IStatEntryModifier modifier, ModificationToken modificationToken);
		void RemoveModifier(IStatEntryModifier modifier, ModificationToken modificationToken);
		void RemoveModifiers(ModificationToken modificationToken);
	}
}