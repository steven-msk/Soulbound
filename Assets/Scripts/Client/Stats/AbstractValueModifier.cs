using System;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public abstract class AbstractValueModifier {
		public readonly StatApplicationType applicationType;
		public abstract bool keepSign { get; }

		public AbstractValueModifier(StatApplicationType applicationType) {
			this.applicationType = applicationType;
		}

		public abstract object GetBoxedValue();

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
