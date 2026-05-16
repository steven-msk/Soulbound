using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics;

namespace SoulboundEngine.Client {
	public struct ClientInit {
		public DebugMetricsService debugMetricsService;
		public LogConsole logConsole;
	}
}
