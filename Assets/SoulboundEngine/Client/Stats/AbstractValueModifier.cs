using SoulboundEngine.Client.ItemSystem;
using System;

#nullable enable

namespace SoulboundEngine.Client.Stats {
	public abstract class AbstractValueModifier {
		public readonly StatApplicationType applicationType;
		public abstract bool keepSign { get; }

		public AbstractValueModifier(StatApplicationType applicationType) {
			this.applicationType = applicationType;
		}

		public abstract object GetBoxedValue();

		public abstract override string ToString();
		public abstract object Clone();
	}

}
