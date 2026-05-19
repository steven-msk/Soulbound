using SoulboundEngine.Client.Debug;
using SoulboundEngine.Client.Debug.Logging.Console;
using SoulboundEngine.Client.Debug.Metrics.View;
using SoulboundEngine.Core.Assets;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class WorldScreen : UxmlScreen {
		private readonly CommandLine commandLine;
		private readonly MetricsHUD metricsHUD;
		private readonly LogConsole logConsole;

		public WorldScreen(CommandLine commandLine, MetricsHUD metricsHUD, LogConsole logConsole) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("WorldScreen"))) {
			this.commandLine = commandLine;
			this.metricsHUD = metricsHUD;
			this.logConsole = logConsole;
		}

		public override bool EscapeReturn => false;

		protected override void OnBind(VisualElement root) {
			this.commandLine.OnBind(root.Q<VisualElement>("CommandLine"));
			this.metricsHUD.OnBind(root.Q<VisualElement>("MetricsHUD"));
			this.logConsole.OnBind(root.Q<VisualElement>("LogConsole"));
		}

		public override void OnDispose(IScreenHandle handle) {
			this.commandLine.Dispose();
		}
	}
}
