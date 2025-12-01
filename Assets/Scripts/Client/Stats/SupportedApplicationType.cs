using System;

namespace SoulboundBackend.Client.Stats {
	[Flags]
	public enum SupportedApplicationType {
		FlatOnly				= 1 << 0,
		PercentageOnly			= 1 << 1,
		FlatAndPercentage		= FlatOnly | PercentageOnly,
	}

	public static class SupportedApplicationTypeValidation {
		public static bool Supports(this SupportedApplicationType type, StatApplicationType test) {
			return type.HasFlag((SupportedApplicationType)(int)test);
		}
	}
}
