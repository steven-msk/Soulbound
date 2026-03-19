using SoulboundBackend.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public sealed record ModificationToken(object? tag = null);

	public interface IStatEntryModifier {
		void Apply(IStatEntry entry, ModificationToken modificationToken);
		void Remove(IStatEntry entry, ModificationToken modificationToken);
	}

	public interface IStatEntryModifier<TValue> : IStatEntryModifier where TValue : struct, IComparable<TValue> {
		void Apply(StatEntry<TValue> entry, ModificationToken modificationToken);
		void Remove(StatEntry<TValue> entry, ModificationToken modificationToken);

		void IStatEntryModifier.Apply(IStatEntry entry, ModificationToken modificationToken) {
			if (entry is StatEntry<TValue> typed) {
				Apply(typed, modificationToken);
			} else {
				Logger.LogError("Mistyped entry in Apply, expected {}", typeof(TValue));
			}
		}

		void IStatEntryModifier.Remove(IStatEntry entry, ModificationToken modificationToken) {
			if (entry is StatEntry<TValue> typed) {
				Remove(typed, modificationToken);
			} else {
				Logger.LogError("Mistyped entry in Remove, expected {}", typeof(TValue));
			}
		}
	}

}
