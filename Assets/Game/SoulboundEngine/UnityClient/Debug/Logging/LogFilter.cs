namespace SoulboundEngine.UnityClient.Debug.Logging {
	using System;

	[Flags]
	public enum LogFilter : int {
		NONE	= 0,
		INFO	= 1 << 0,
		WARNING = 1 << 1,
		ERROR	= 1 << 2,
		FATAL	= 1 << 3,
		ALL = INFO | WARNING | ERROR | FATAL
	}
}
