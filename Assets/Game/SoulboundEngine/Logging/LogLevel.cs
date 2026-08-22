namespace SoulboundEngine.Logging {
	using System;

	[Flags]
	public enum LogLevel {
		Info		= 1 << 0,
		Warning		= 1 << 1,
		Error		= 1 << 2,
		Fatal		= 1 << 3
	}
}
