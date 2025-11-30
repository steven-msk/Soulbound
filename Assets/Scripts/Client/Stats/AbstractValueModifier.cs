using System;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public abstract class AbstractValueModifier : IStatEntryModifier {
		public readonly IStatDefinition statDefinition;
		public readonly StatApplicationType applicationType;
		public abstract bool keepSign { get; }

		public AbstractValueModifier(IStatDefinition statDefinition, StatApplicationType applicationType) {
			this.statDefinition = statDefinition;
		}

		public abstract object GetBoxedValue();

		public abstract void Apply(IStatEntry entry, ModificationToken modificationToken);
		public abstract void Remove(IStatEntry entry, ModificationToken modificationToken);

		public abstract override string ToString();
		public abstract override int GetHashCode();
		public abstract object Clone();

		public override bool Equals(object other) {
			return other is AbstractValueModifier stat && stat.GetHashCode() == this.GetHashCode();
		}

		public static bool operator ==(AbstractValueModifier first, AbstractValueModifier second) {
			return first.Equals(second);
		}

		public static bool operator !=(AbstractValueModifier first, AbstractValueModifier second) {
			return !(first == second);
		}
	}

}
